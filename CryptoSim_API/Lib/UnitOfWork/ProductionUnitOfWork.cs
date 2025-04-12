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

		private readonly IServiceScopeFactory _scopeFactory;

		public ICryptoRespository CryptoRepository { get; }
		public IProfitRepository ProfitRepository { get; }
		public ITradeRepository TradeRepository { get; }
		public ITransactionRepository TransactionRepository { get; }
		public IWalletRepository WalletRepository { get; }
		public IUserRepository UserRepository { get; }

		public ProductionUnitOfWork(IServiceScopeFactory scopeFactory,CryptoContext dbContext, IMemoryCache cache,ITradeService tradeManager ,IProfitService profitManager, IUserService userManager, ICryptoService cryptoManager, IWalletService walletManager, ITransactionService transactionManager)
		{
			_dbContext = dbContext;
			_cache = cache;

			_userManager = userManager;
			_cryptoManager = cryptoManager;
			_walletManager = walletManager;
			_transactionManager = transactionManager;
			_profitManager = profitManager;
			_tradeManager = tradeManager;

			_scopeFactory = scopeFactory;

			CryptoRepository = new CryptoRepository(_scopeFactory);
			ProfitRepository = new ProfitRepository(_scopeFactory);
			TradeRepository = new TradeRepository(_scopeFactory);
			TransactionRepository = new TransactionRepository(_scopeFactory);
			WalletRepository = new WalletRepository(_scopeFactory);
			UserRepository = new UserRepository(_scopeFactory);
		}
		
		public async Task Save()
		{
			var transaction = _dbContext.Database.BeginTransaction();
			await _dbContext.SaveChangesAsync();
			transaction.Commit();
		}
	}
	
}

