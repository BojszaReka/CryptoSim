using CryptoSim_API.Lib.Interfaces.Service;
using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using CryptoSim_Lib.Models;
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
			var _userManager = scope.ServiceProvider.GetRequiredService<IUserService>();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<ICryptoService>();
			var _walletManager = scope.ServiceProvider.GetRequiredService<IWalletService>();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<ICryptoItemService>();
			var _giftManager = scope.ServiceProvider.GetRequiredService<IGiftService>();

			DetailedUserProfitDTO userProfit = new DetailedUserProfitDTO();
			userProfit.UserName = await _userManager.getUserName(userId);

			List<CryptoItem> ci = new List<CryptoItem>();

			Wallet wallet = await _walletManager.GetWalletByUserId(userId);
			var walletId = wallet.Id.ToString();
			if (await _walletManager.doesWalletExists(walletId))
			{
				var cryptoItems = await _cryptoItemManager.GetItemsWith(walletId);
				List<CryptoItem> cryptoItemsList = new List<CryptoItem>();
				foreach (var cryptoItem in cryptoItems)
				{
					var crypto = await _cryptoManager.GetCrypto(cryptoItem.CryptoId.ToString());
					if (!crypto.isDeleted)
					{
						cryptoItemsList.Add(cryptoItem);
					}

				}
				ci = cryptoItemsList;
			}
			else
			{
				scope.Dispose();
				throw new Exception("The user's wallet does not exists");
			}


			if (ci == null || !ci.Any())
			{
				throw new Exception("No crypto items found for the user");
			}
			foreach (var cryptoItem in ci)
			{
				var cryptoExists = await _cryptoManager.doesCryptoExists(cryptoItem.CryptoId.ToString());
				double giftedQuantity = await _giftManager.GetGiftedQuantity(cryptoItem.CryptoId.ToString(), userId);
				if (cryptoExists) {
					double currentrate = await _cryptoManager.GetCurrentRate(cryptoItem.CryptoId);
					if (currentrate != 0)
					{
						ProfitItem pi = new ProfitItem();
						pi.CryptoName = await _cryptoManager.GetCryptoName(cryptoItem.CryptoId); 
						pi.Profit = (currentrate - cryptoItem.BoughtAtRate) * (cryptoItem.Quantity - giftedQuantity);
						userProfit.Profits.Add(pi); 
					}
					else
					{
						throw new Exception($"Crypto with id {cryptoItem.CryptoId} does not have rate");
					}
				}
			}
			scope.Dispose();
			return userProfit;
		}
	}
}
