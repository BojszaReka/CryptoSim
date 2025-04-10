﻿using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using CryptoSim_Lib.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Transactions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Services
{
	public class TradeManagerService : ITradeService
	{
		private readonly IUserService _userManager;
		private readonly ICryptoService _cryptoManager;
		private readonly IWalletService _walletManager;
		private readonly ITransactionService _transactionManager;

		public TradeManagerService(IUserService userManager, ICryptoService cryptoManager, IWalletService walletManager, ITransactionService transactionManager)
		{
			_userManager = userManager;
			_cryptoManager = cryptoManager;
			_walletManager = walletManager;
			_transactionManager = transactionManager;
		}
		//TODO: implement this method in the other services as well

		public async Task<string> BuyCrypto(TradeRequestDTO tradeRequest)
		{

			if (await _cryptoManager.doesCryptoExists(tradeRequest.CryptoId.ToString()))
			{
				if(await _userManager.doesUserExists(tradeRequest.UserId.ToString()))
				{
					var crypto = await _cryptoManager.GetCrypto(tradeRequest.CryptoId.ToString());
					if(crypto.Quantity < tradeRequest.Quantity)
					{
						throw new Exception("Not enough crypto in the market");
					}
					double cost = tradeRequest.Quantity * crypto.CurrentPrice;
					if (await _walletManager.doesUserHasBalance(tradeRequest.UserId.ToString(), cost))
					{
						await _cryptoManager.DecreaseCryptoQuantity(tradeRequest.CryptoId.ToString(), tradeRequest.Quantity);
						await _walletManager.DecreaseUserBalance(tradeRequest.UserId.ToString(), cost);
						await _walletManager.AddCryptoToUserWallet(tradeRequest.UserId.ToString(), tradeRequest.CryptoId.ToString(), tradeRequest.Quantity);
						
						NewTransactionDTO nt = new NewTransactionDTO
						{
							UserId = tradeRequest.UserId,
							CryptoId = tradeRequest.CryptoId,
							Quantity = tradeRequest.Quantity,
							Price = crypto.CurrentPrice,
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

			if (!await _userManager.doesUserExists(userId)) throw new Exception("The user with the provided ID does not exist");
			UserPortfolioDTO userPortfolio = new UserPortfolioDTO();
			userPortfolio.UserName = await _userManager.getUserName(userId);
			userPortfolio.Cryptos = await _walletManager.getUserWalletAsPortfolioList(userId);
			return userPortfolio;
		}
	}
}
