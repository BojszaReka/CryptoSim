using CryptoSim_API.Lib.RepositoryIntefaces;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Repositories
{
	public class WalletRepository : IWalletRepository
	{
		private readonly CryptoContext _dbContext;
		private readonly IDistributedCache _cache;
		WalletManagerService _walletManager;
		public WalletRepository(CryptoContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
			_walletManager = new WalletManagerService(_dbContext, _cache);
		}
		public async Task<string> DeleteWallet(string userId)
		{
			return await _walletManager.DeleteWallet(userId);
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
