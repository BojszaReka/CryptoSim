using CryptoSim_Lib.Enums;
using Microsoft.Extensions.Caching.Distributed;
using System.Transactions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Services
{
	public class TradeManagerService
	{
		private readonly CryptoContext _dbContext;
		private readonly IDistributedCache _cache;

		private UserManagerService userManager;
		private CryptoManagerService cryptoManager;
		private WalletManagerService walletManager;
		private TransactionManagerService transactionManager;
		public TradeManagerService(CryptoContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;

			userManager = new UserManagerService(dbContext, cache);
			cryptoManager = new CryptoManagerService(dbContext, cache);
			walletManager = new WalletManagerService(dbContext, cache);
			transactionManager = new TransactionManagerService(dbContext, cache);
		}
		public async Task<string> BuyCrypto(TradeRequestDTO tradeRequest)
		{
			if(await cryptoManager.doesCryptoExists(tradeRequest.CryptoId.ToString()))
			{
				if(await userManager.doesUserExists(tradeRequest.UserId.ToString()))
				{
					var crypto = await cryptoManager.GetCrypto(tradeRequest.CryptoId.ToString());
					if(crypto.Quantity < tradeRequest.Quantity)
					{
						return "Not enough crypto in the market";
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
					else { return "The user does not have enough balance"; }
				}
				else { return "The user with the provided ID does not exist"; }
			}
			else { return "The crypto currency with the provided ID does not exist"; }
		}

		public async Task<string> SellCrypto(TradeRequestDTO tradeRequest)
		{
			if (await cryptoManager.doesCryptoExists(tradeRequest.CryptoId.ToString()))
			{
				if (await userManager.doesUserExists(tradeRequest.UserId.ToString()))
				{
					var crypto = await cryptoManager.GetCrypto(tradeRequest.CryptoId.ToString());
					double cost = tradeRequest.Quantity * crypto.CurrentPrice;

					var hasEnoughCrypto = await walletManager.doesUserHasCryptoBalance(tradeRequest.UserId.ToString(), tradeRequest.CryptoId.ToString(), tradeRequest.Quantity);
					if(!hasEnoughCrypto) { return "The user does not have enough of the crypto currency to sell";}

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
				else { return "The user with the provided ID does not exist"; }
			}
			else { return "The crypto currency with the provided ID does not exist"; }
		}

		public async Task<UserPortfolioDTO> getUserPortfolio(string userId)
		{
			if (!await userManager.doesUserExists(userId)) throw new Exception("The user with the provided ID does not exist");
			UserPortfolioDTO userPortfolio = new UserPortfolioDTO();
			userPortfolio.UserName = await userManager.getUserName(userId);
			userPortfolio.Cryptos = await walletManager.getUserWalletAsPortfolioList(userId);
			return userPortfolio;
		}
	}
}
