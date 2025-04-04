using Microsoft.Extensions.Caching.Distributed;
using System.Net.WebSockets;

namespace CryptoSim_API.Lib.Services
{
	public class ProfitManagerService
	{
		private readonly CryptoContext _dbContext;
		private readonly IDistributedCache _cache;

		private UserManagerService userManager;
		private WalletManagerService walletManager;
		private CryptoManagerService cryptoManager;

		public ProfitManagerService(CryptoContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;

			userManager = new UserManagerService(dbContext, cache);
			walletManager = new WalletManagerService(dbContext, cache);
			cryptoManager = new CryptoManagerService(dbContext, cache);
		}

		public async Task<UserProfitDTO> GetUserProfit(string userId)
		{
			DetailedUserProfitDTO userProfit = await GetDetailedUserProfit(userId);
			double totalProfit = 0;
			foreach (var item in userProfit.Profits)
			{
				totalProfit += item.Profit;
			}
			return new UserProfitDTO
			{
				UserName = userProfit.UserName,
				TotalProfit = totalProfit
			};
		}

		public async Task<DetailedUserProfitDTO> GetDetailedUserProfit(string userId)
		{
			DetailedUserProfitDTO userProfit = new DetailedUserProfitDTO();
			userProfit.UserName = await userManager.getUserName(userId);

			var ci = await walletManager.getCryptoItems(userId);
			foreach (var cryptoItem in ci)
			{
				double currentrate = await cryptoManager.GetCurrentRate(cryptoItem.CryptoId);
				if (currentrate != 0)
				{
					ProfitItem pi = new ProfitItem();
					pi.CryptoName = await cryptoManager.GetCryptoName(cryptoItem.Id);
					pi.Profit = (currentrate - cryptoItem.BoughtAtRate) * cryptoItem.Quantity;
					userProfit.Profits.Add(pi);
				}
				else
				{
					throw new Exception($"Crypto with id {cryptoItem.CryptoId} does not have rate");
				}
			}

			return userProfit;
		}
	}
}
