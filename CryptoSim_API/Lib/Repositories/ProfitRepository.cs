using CryptoSim_API.Lib.Interfaces;
using CryptoSim_API.Lib.Interfaces.RepositoryIntefaces;
using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Caching.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Repositories
{
	public class ProfitRepository : IProfitRepository
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;
		public ProfitRepository(IServiceScopeFactory scopeFactory, CryptoContext dbContext, IMemoryCache cache)
		{
			_scopeFactory = scopeFactory;
			_dbContext = dbContext;
			_cache = cache;
		}
		private IProfitService GetService()
		{
			var scope = _scopeFactory.CreateScope();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<IProfitService>();
			return _cryptoManager;
		}

		public async Task<DetailedUserProfitDTO> GetDetailedUserProfit(string userId)
		{
			var _profitManager = GetService();
			return await _profitManager.GetDetailedUserProfit(userId);
		}

		public async Task<UserProfitDTO> GetUserProfit(string userId)
		{
			var _profitManager = GetService();
			return await _profitManager.GetUserProfit(userId);
		}
	}
}
