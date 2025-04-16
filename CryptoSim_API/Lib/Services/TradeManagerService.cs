using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using CryptoSim_Lib.Enums;
using CryptoSim_Lib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Transactions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Services
{
	public class TradeManagerService : ITradeService
	{
		private readonly IServiceScopeFactory _scopeFactory;

		public TradeManagerService(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
		}

		public async Task<string> BuyCrypto(TradeRequestDTO tradeRequest)
		{
			using var scope = _scopeFactory.CreateScope();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<ICryptoService>();
			var _userManager = scope.ServiceProvider.GetRequiredService<IUserService>();
			var _walletManager = scope.ServiceProvider.GetRequiredService<IWalletService>();
			var _transactionManager = scope.ServiceProvider.GetRequiredService<ITransactionService>();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<ICryptoItemService>();

			if (await _cryptoManager.doesCryptoExists(tradeRequest.CryptoId.ToString()))
			{
				if(await _userManager.doesUserExists(tradeRequest.UserId.ToString()))
				{
					if(!await _cryptoManager.isEnoughCrypto(tradeRequest.CryptoId.ToString(), tradeRequest.Quantity))
					{
						throw new Exception("Not enough crypto in the market");
					}
					var current = await _cryptoManager.GetCurrentRate(tradeRequest.CryptoId);
					double cost = tradeRequest.Quantity * current;
					if (await _walletManager.doesUserHasBalance(tradeRequest.UserId.ToString(), cost))
					{
						await _cryptoManager.DecreaseCryptoQuantity(tradeRequest.CryptoId.ToString(), tradeRequest.Quantity);
						await _walletManager.DecreaseUserBalance(tradeRequest.UserId.ToString(), cost);
						//await _walletManager.AddCryptoToUserWallet(tradeRequest.UserId.ToString(), tradeRequest.CryptoId.ToString(), tradeRequest.Quantity);

						var wallet = await _walletManager.GetWalletByUserId(tradeRequest.UserId.ToString());
						var cryptoItems = await _cryptoItemManager.GetItemsWith(wallet.Id.ToString());
						var cryptoItem = cryptoItems.Where(c => c.CryptoId.ToString().Equals(tradeRequest.CryptoId.ToString())).FirstOrDefault();
						if (cryptoItem != null)
						{
							cryptoItem.Quantity += tradeRequest.Quantity;
							await _cryptoItemManager.UpdateCryptoItem(cryptoItem);
						}
						else
						{
							var rate = await _cryptoManager.GetCurrentRate(tradeRequest.CryptoId);
							CryptoItem newCryptoItem = new CryptoItem
							{
								Id = Guid.NewGuid(),
								CryptoId = tradeRequest.CryptoId,
								WalletId = wallet.Id,
								Quantity = tradeRequest.Quantity,
								BoughtAtRate = rate
							};
							await _cryptoItemManager.CreateCryptoItem(newCryptoItem);
						}

						NewTransactionDTO nt = new NewTransactionDTO
						{
							UserId = tradeRequest.UserId,
							CryptoId = tradeRequest.CryptoId,
							Quantity = tradeRequest.Quantity,
							Price = current,
							Type = ETransactionType.Buy
						};
						await _transactionManager.CreateTransaction(nt);
						return "Trade created successfully";
					}
					else { throw new Exception($"The user does not have enough balance, required: {cost}"); }
				}
				else { throw new Exception("The user with the provided ID does not exist"); }
			}
			else { throw new Exception("The crypto currency with the provided ID does not exist"); }
			
		}

		public async Task<string> SellCrypto(TradeRequestDTO tradeRequest)
		{
			using var scope = _scopeFactory.CreateScope();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<ICryptoService>();
			var _userManager = scope.ServiceProvider.GetRequiredService<IUserService>();
			var _walletManager = scope.ServiceProvider.GetRequiredService<IWalletService>();
			var _transactionManager = scope.ServiceProvider.GetRequiredService<ITransactionService>();

			if (await _cryptoManager.doesCryptoExists(tradeRequest.CryptoId.ToString()))
			{
				if (await _userManager.doesUserExists(tradeRequest.UserId.ToString()))
				{
					var crypto = await _cryptoManager.GetCrypto(tradeRequest.CryptoId.ToString());
					double cost = tradeRequest.Quantity * crypto.CurrentPrice;

					var hasEnoughCrypto = await _walletManager.doesUserHasCryptoBalance(tradeRequest.UserId.ToString(), tradeRequest.CryptoId.ToString(), tradeRequest.Quantity);
					if(!hasEnoughCrypto) { throw new Exception("The user does not have enough of the crypto currency to sell");}

					await _cryptoManager.IncreaseCryptoQuantity(tradeRequest.CryptoId.ToString(), tradeRequest.Quantity);
					await _walletManager.IncreaseUserBalance(tradeRequest.UserId.ToString(), cost);
					await _walletManager.RemoveCryptoFromUserWallet(tradeRequest.UserId.ToString(), tradeRequest.CryptoId.ToString(), tradeRequest.Quantity);

					NewTransactionDTO nt = new NewTransactionDTO
					{
						UserId = tradeRequest.UserId,
						CryptoId = tradeRequest.CryptoId,
						Quantity = tradeRequest.Quantity,
						Price = crypto.CurrentPrice,
						Type = ETransactionType.Sell
					};
					await _transactionManager.CreateTransaction(nt);
					return "Trade created successfully";
				}
				else { throw new Exception("The user with the provided ID does not exist"); }
			}
			else { throw new Exception("The crypto currency with the provided ID does not exist"); }
		}

		public async Task<UserPortfolioDTO> getUserPortfolio(string userId)
		{
			using var scope = _scopeFactory.CreateScope();
			var _userManager = scope.ServiceProvider.GetRequiredService<IUserService>();
			var _walletManager = scope.ServiceProvider.GetRequiredService<IWalletService>();

			if (!await _userManager.doesUserExists(userId)) throw new Exception("The user with the provided ID does not exist");
			UserPortfolioDTO userPortfolio = new UserPortfolioDTO();
			userPortfolio.UserName = await _userManager.getUserName(userId);
			userPortfolio.Cryptos = await _walletManager.getUserWalletAsPortfolioList(userId);
			return userPortfolio;
		}

		
	}
}
