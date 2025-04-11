using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Net.WebSockets;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Services
{
	public class ProfitManagerService : IProfitService
	{
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;

		private readonly IServiceScopeFactory _scopeFactory;

		public ProfitManagerService(CryptoContext dbContext, IMemoryCache cache, IServiceScopeFactory scopeFactory)
		{
			_dbContext = dbContext;
			_cache = cache;
			_scopeFactory = scopeFactory;
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
			using var scope = _scopeFactory.CreateScope();
			var _userManager = scope.ServiceProvider.GetRequiredService<UserManagerService>();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<CryptoManagerService>();
			var _walletManager = scope.ServiceProvider.GetRequiredService<WalletManagerService>();

			DetailedUserProfitDTO userProfit = new DetailedUserProfitDTO();
			userProfit.UserName = await _userManager.getUserName(userId);

			var ci = await _walletManager.getCryptoItems(userId);
			if (ci == null || !ci.Any())
			{
				throw new Exception("No crypto items found for the user");
			}
			foreach (var cryptoItem in ci)
			{
				var cryptoExists = await _cryptoManager.doesCryptoExists(cryptoItem.CryptoId.ToString());
				if (cryptoExists) {
					double currentrate = await _cryptoManager.GetCurrentRate(cryptoItem.CryptoId);
					if (currentrate != 0)
					{
						ProfitItem pi = new ProfitItem();
						pi.CryptoName = await _cryptoManager.GetCryptoName(cryptoItem.Id);
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
