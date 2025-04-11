using CryptoSim_API.Lib.Interfaces.RepositoryIntefaces;
using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Caching.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.UnitOfWork
{
	public class ProductionUnitOfWork : IUnitOfWork
	{
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;

		private readonly IUserService _userManager;
		private readonly ICryptoService _cryptoManager;
		private readonly IWalletService _walletManager;
		private readonly ITransactionService _transactionManager;
		private readonly IProfitService _profitManager;
		private readonly ITradeService _tradeManager;

		public ICryptoRespository CryptoRepository { get; }
		public IProfitRepository ProfitRepository { get; }
		public ITradeRepository TradeRepository { get; }
		public ITransactionRepository TransactionRepository { get; }
		public IWalletRepository WalletRepository { get; }
		public IUserRepository UserRepository { get; }

		public ProductionUnitOfWork(CryptoContext dbContext, IMemoryCache cache,ITradeService tradeManager ,IProfitService profitManager, IUserService userManager, ICryptoService cryptoManager, IWalletService walletManager, ITransactionService transactionManager)
		{
			_dbContext = dbContext;
			_cache = cache;

			_userManager = userManager;
			_cryptoManager = cryptoManager;
			_walletManager = walletManager;
			_transactionManager = transactionManager;
			_profitManager = profitManager;
			_tradeManager = tradeManager;

			CryptoRepository = new CryptoRepository(_dbContext, _cache);
			ProfitRepository = new ProfitRepository(_dbContext, _cache, _profitManager);
			TradeRepository = new TradeRepository(_dbContext, _cache, _tradeManager);
			TransactionRepository = new TransactionRepository(_dbContext, _cache);
			WalletRepository = new WalletRepository(_dbContext, _cache, _walletManager);
			UserRepository = new UserRepository(_dbContext, cache, _userManager);
		}
		
		public async Task Save()
		{
			var transaction = _dbContext.Database.BeginTransaction();
			await _dbContext.SaveChangesAsync();
			transaction.Commit();
		}
	}
	
}

