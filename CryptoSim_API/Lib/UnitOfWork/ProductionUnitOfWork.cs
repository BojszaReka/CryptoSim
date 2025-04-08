using CryptoSim_API.Lib.RepositoryIntefaces;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.UnitOfWork
{
	public class ProductionUnitOfWork : IUnitOfWork
	{
		private readonly CryptoContext _dbContext;
		private readonly IDistributedCache _cache;

		public ICryptoRespository CryptoRepository { get; }
		public IProfitRepository ProfitRepository { get; }
		public ITradeRepository TradeRepository { get; }
		public ITransactionRepository TransactionRepository { get; }
		public IWalletRepository WalletRepository { get; }
		public IUserRepository UserRepository { get; }

		public ProductionUnitOfWork(CryptoContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
			CryptoRepository = new CryptoRepository(_dbContext, _cache);
			ProfitRepository = new ProfitRepository(_dbContext, _cache);
			TradeRepository = new TradeRepository(_dbContext, _cache);
			TransactionRepository = new TransactionRepository(_dbContext, _cache);
			WalletRepository = new WalletRepository(_dbContext, _cache);
			UserRepository = new UserRepository(_dbContext, cache);
		}
		
		public async Task Save()
		{
			var transaction = _dbContext.Database.BeginTransaction();
			await _dbContext.SaveChangesAsync();
			transaction.Commit();
		}
	}
	
}

