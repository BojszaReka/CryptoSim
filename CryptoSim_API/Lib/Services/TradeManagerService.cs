using CryptoSim_Lib.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Transactions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Services
{
	public class TradeManagerService
	{
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;

		public TradeManagerService(CryptoContext dbContext, IMemoryCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
		}
		public async Task<string> BuyCrypto(TradeRequestDTO tradeRequest)
		{
			UserManagerService userManager = new UserManagerService(_dbContext, _cache);
			CryptoManagerService cryptoManager = new CryptoManagerService(_dbContext, _cache);
			WalletManagerService walletManager = new WalletManagerService(_dbContext, _cache);
			TransactionManagerService transactionManager = new TransactionManagerService(_dbContext, _cache);

			if (await cryptoManager.doesCryptoExists(tradeRequest.CryptoId.ToString()))
			{
				if(await userManager.doesUserExists(tradeRequest.UserId.ToString()))
				{
					var crypto = await cryptoManager.GetCrypto(tradeRequest.CryptoId.ToString());
					if(crypto.Quantity < tradeRequest.Quantity)
					{
						throw new Exception("Not enough crypto in the market");
					}
					double cost = tradeRequest.Quantity * crypto.CurrentPrice;
					if (await walletManager.doesUserHasBalance(tradeRequest.UserId.ToString(), cost))
					{
						await cryptoManager.DecreaseCryptoQuantity(tradeRequest.CryptoId.ToString(), tradeRequest.Quantity);
						await walletManager.DecreaseUserBalance(tradeRequest.UserId.ToString(), cost);
						await walletManager.AddCryptoToUserWallet(tradeRequest.UserId.ToString(), tradeRequest.CryptoId.ToString(), tradeRequest.Quantity);
						
						NewTransactionDTO nt = new NewTransactionDTO
						{
							UserId = tradeRequest.UserId,
							CryptoId = tradeRequest.CryptoId,
							Quantity = tradeRequest.Quantity,
							Price = crypto.CurrentPrice,
							Type = ETransactionType.Buy
						};
						await transactionManager.CreateTransaction(nt);
						return "Trade created successfully";
					}
					else { throw new Exception("The user does not have enough balance"); }
				}
				else { throw new Exception("The user with the provided ID does not exist"); }
			}
			else { throw new Exception("The crypto currency with the provided ID does not exist"); }
			
		}

		public async Task<string> SellCrypto(TradeRequestDTO tradeRequest)
		{
			UserManagerService userManager = new UserManagerService(_dbContext, _cache);
			CryptoManagerService cryptoManager = new CryptoManagerService(_dbContext, _cache);
			WalletManagerService walletManager = new WalletManagerService(_dbContext, _cache);
			TransactionManagerService transactionManager = new TransactionManagerService(_dbContext, _cache);

			if (await cryptoManager.doesCryptoExists(tradeRequest.CryptoId.ToString()))
			{
				if (await userManager.doesUserExists(tradeRequest.UserId.ToString()))
				{
					var crypto = await cryptoManager.GetCrypto(tradeRequest.CryptoId.ToString());
					double cost = tradeRequest.Quantity * crypto.CurrentPrice;

					var hasEnoughCrypto = await walletManager.doesUserHasCryptoBalance(tradeRequest.UserId.ToString(), tradeRequest.CryptoId.ToString(), tradeRequest.Quantity);
					if(!hasEnoughCrypto) { throw new Exception("The user does not have enough of the crypto currency to sell");}

					await cryptoManager.IncreaseCryptoQuantity(tradeRequest.CryptoId.ToString(), tradeRequest.Quantity);
					await walletManager.IncreaseUserBalance(tradeRequest.UserId.ToString(), cost);
					await walletManager.RemoveCryptoFromUserWallet(tradeRequest.UserId.ToString(), tradeRequest.CryptoId.ToString(), tradeRequest.Quantity);

					NewTransactionDTO nt = new NewTransactionDTO
					{
						UserId = tradeRequest.UserId,
						CryptoId = tradeRequest.CryptoId,
						Quantity = tradeRequest.Quantity,
						Price = crypto.CurrentPrice,
						Type = ETransactionType.Sell
					};
					await transactionManager.CreateTransaction(nt);
					return "Trade created successfully";
				}
				else { throw new Exception("The user with the provided ID does not exist"); }
			}
			else { throw new Exception("The crypto currency with the provided ID does not exist"); }
		}

		public async Task<UserPortfolioDTO> getUserPortfolio(string userId)
		{
			WalletManagerService walletManager = new WalletManagerService(_dbContext, _cache);
			UserManagerService userManager = new UserManagerService(_dbContext, _cache);

			if (!await userManager.doesUserExists(userId)) throw new Exception("The user with the provided ID does not exist");
			UserPortfolioDTO userPortfolio = new UserPortfolioDTO();
			userPortfolio.UserName = await userManager.getUserName(userId);
			userPortfolio.Cryptos = await walletManager.getUserWalletAsPortfolioList(userId);
			return userPortfolio;
		}
	}
}
