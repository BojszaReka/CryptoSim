using Microsoft.Extensions.Caching.Distributed;

namespace CryptoSim_API.Lib.Services
{
	public class WalletManagerService
	{
		private readonly CryptoContext _dbContext;
		private readonly IDistributedCache _cache;
		public WalletManagerService(CryptoContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
		}
		//TODO: Implement wallet manager service

		public async Task<IEnumerable<CryptoItem>> getCryptoItems(string walletId)
		{
			throw new NotImplementedException("WalletManagerService.getCryptoItems not implemented");
		}

		public async Task<bool> doesUserHasBalance(string userId)
		{
			throw new NotImplementedException("WalletManagerService.doesUserHasBalance not implemented");
		}

		public async Task DecreaseUserBalance(string v, double cost)
		{
			throw new NotImplementedException("WalletManagerService.DecreaseUserBalance not implemented");
		}

		public async Task AddCryptoToUserWallet(string v1, string v2, int quantity)
		{
			throw new NotImplementedException("WalletManagerService.AddCryptoToUserWallet not implemented");
		}

		public async Task IncreaseUserBalance(string v, double cost)
		{
			throw new NotImplementedException("WalletManagerService.IncreaseUserBalance not implemented");
		}

		public async Task RemoveCryptoFromUserWallet(string v1, string v2, int quantity)
		{
			throw new NotImplementedException("WalletManagerService.RemoveCryptoFromUserWallet not implemented");
		}

		public async Task<List<PortfolioItem>> getUserWalletAsPortfolioList(string userId)
		{
			throw new NotImplementedException("WalletManagerService.getUserWalletAsPortfolioList not implemented");
		}
	}
}
