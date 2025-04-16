using CryptoSim_API.Lib.Interfaces.RepositoryIntefaces;
using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Caching.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Repositories
{
	public class TradeRepository : ITradeRepository
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;
		public TradeRepository(IServiceScopeFactory scopeFactory, CryptoContext dbContext, IMemoryCache cache)
		{
			_scopeFactory = scopeFactory;
			_dbContext = dbContext;
			_cache = cache;
		}

		private ITradeService GetService()
		{
			var scope = _scopeFactory.CreateScope();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<ITradeService>();
			return _cryptoManager;
		}
		public async Task<string> BuyCrypto(TradeRequestDTO tradeRequest)
		{
			var _tradeManager = GetService();
			return await _tradeManager.BuyCrypto(tradeRequest);
		}
		public async Task<string> SellCrypto(TradeRequestDTO tradeRequest)
		{
			var _tradeManager = GetService();
			return await _tradeManager.SellCrypto(tradeRequest);
		}
		public async Task<UserPortfolioDTO> getUserPortfolio(string userId)
		{
			var _tradeManager = GetService();
			return await _tradeManager.getUserPortfolio(userId);
		}
	
	}
}
