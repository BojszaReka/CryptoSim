using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Net.WebSockets;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Services
{
	public class ProfitManagerService
	{
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;

		

		public ProfitManagerService(CryptoContext dbContext, IMemoryCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
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
			UserManagerService userManager = new UserManagerService(_dbContext, _cache);
			WalletManagerService walletManager = new WalletManagerService(_dbContext, _cache);
			CryptoManagerService cryptoManager = new CryptoManagerService(_dbContext, _cache);

			DetailedUserProfitDTO userProfit = new DetailedUserProfitDTO();
			userProfit.UserName = await userManager.getUserName(userId);

			var ci = await walletManager.getCryptoItems(userId);
			if (ci == null || !ci.Any())
			{
				throw new Exception("No crypto items found for the user");
			}
			foreach (var cryptoItem in ci)
			{
				var cryptoExists = await cryptoManager.doesCryptoExists(cryptoItem.CryptoId.ToString());
				if (cryptoExists) {
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
			}
			return userProfit;
		}
	}
}
