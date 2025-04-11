using CryptoSim_API.Lib.Interfaces;
using CryptoSim_API.Lib.Interfaces.RepositoryIntefaces;
using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Caching.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Repositories
{
	public class ProfitRepository : IProfitRepository
	{
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;
		IProfitService _profitManager;
		public ProfitRepository(CryptoContext dbContext, IMemoryCache cache, IProfitService profitservice)
		{
			_dbContext = dbContext;
			_cache = cache;
			_profitManager = profitservice;
		}
		public async Task<DetailedUserProfitDTO> GetDetailedUserProfit(string userId)
		{
			return await _profitManager.GetDetailedUserProfit(userId);
		}

		public async Task<UserProfitDTO> GetUserProfit(string userId)
		{
			return await _profitManager.GetUserProfit(userId);
		}
	}
}
