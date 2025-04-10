﻿using CryptoSim_API.Lib.Interfaces.RepositoryIntefaces;
using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Caching.Memory;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Repositories
{
	public class WalletRepository : IWalletRepository
	{
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;
		IWalletService _walletManager;
		public WalletRepository(CryptoContext dbContext, IMemoryCache cache, IWalletService walletService)
		{
			_dbContext = dbContext;
			_cache = cache;
			_walletManager = walletService;
		}
		public async Task<string> DeleteWallet(string userId)
		{
			return await _walletManager.DeleteWalletData(userId);
		}

		public async Task<WalletViewDTO> GetWallet(string userId)
		{
			return await _walletManager.GetWalletViewDTO(userId);
		}

		public async Task<string> UpdateWallet(WalletUpdateDTO wallet)
		{
			return await _walletManager.UpdateWallet(wallet);
		}
	}
}
