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

		public async Task<IEnumerable<CryptoItem>> getCryptoItems(string Id)
		{
			throw new NotImplementedException("WalletManagerService.getCryptoItems not implemented");
		}
	}
}
