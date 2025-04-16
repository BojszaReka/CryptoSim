using CryptoSim_API.Lib.Interfaces.RepositoryIntefaces;
using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Caching.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.UnitOfWork
{
	public class ProductionUnitOfWork : IUnitOfWork
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;

		public ICryptoRespository CryptoRepository { get; }
		public IProfitRepository ProfitRepository { get; }
		public ITradeRepository TradeRepository { get; }
		public ITransactionRepository TransactionRepository { get; }
		public IWalletRepository WalletRepository { get; }
		public IUserRepository UserRepository { get; }


		public ProductionUnitOfWork(IServiceScopeFactory scopeFactory, CryptoContext dbContext, IMemoryCache cache)
		{
			_scopeFactory = scopeFactory;
			_dbContext = dbContext;
			_cache = cache;

			CryptoRepository = new CryptoRepository(_scopeFactory, dbContext, cache);
			ProfitRepository = new ProfitRepository(_scopeFactory, dbContext, cache);
			TradeRepository = new TradeRepository(_scopeFactory, dbContext, cache);
			TransactionRepository = new TransactionRepository(_scopeFactory, dbContext, cache);
			WalletRepository = new WalletRepository(_scopeFactory, dbContext, cache);
			UserRepository = new UserRepository(_scopeFactory, dbContext, cache);
			
		}

		public void Dispose()
		{
			_dbContext.Dispose();
		}
	}
	
}

