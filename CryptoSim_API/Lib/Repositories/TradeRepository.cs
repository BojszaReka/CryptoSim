using CryptoSim_API.Lib.Interfaces.RepositoryIntefaces;
using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Caching.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Repositories
{
	public class TradeRepository : ITradeRepository
	{
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;
		ITradeService _tradeManager;
		public TradeRepository(CryptoContext dbContext, IMemoryCache cache, ITradeService tradeService)
		{
			_dbContext = dbContext;
			_cache = cache;
			_tradeManager = tradeService;
		}
		public async Task<string> BuyCrypto(TradeRequestDTO tradeRequest)
		{
			return await _tradeManager.BuyCrypto(tradeRequest);
		}
		public async Task<string> SellCrypto(TradeRequestDTO tradeRequest)
		{
			return await _tradeManager.SellCrypto(tradeRequest);
		}
		public async Task<UserPortfolioDTO> getUserPortfolio(string userId)
		{
			return await _tradeManager.getUserPortfolio(userId);
		}
	
	}
}
