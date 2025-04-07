#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member


namespace CryptoSim_API.Lib.Services
{
	public class CryptoItemManagerService
	{
		private readonly CryptoContext _dbContext;
		private readonly IDistributedCache _cache;
		public CryptoItemManagerService(CryptoContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
		}

		internal async Task<IEnumerable<CryptoItem>> GetItemsWith(string walletId)
		{
			throw new NotImplementedException();
		}
	}
}
