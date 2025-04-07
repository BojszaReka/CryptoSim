using CryptoSim_API.Lib.RepositoryIntefaces;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Repositories
{
	public class TradeRepository : ITradeRepository
	{
		private readonly CryptoContext _dbContext;
		private readonly IDistributedCache _cache;
		TradeManagerService _tradeManager;
		public TradeRepository(CryptoContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
			_tradeManager = new TradeManagerService(_dbContext, _cache);
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
