using CryptoSim_API.Lib.Interfaces.RepositoryIntefaces;
using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Caching.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.UnitOfWork
{
	public class ProductionUnitOfWork : IUnitOfWork
	{
		private readonly IServiceScopeFactory _scopeFactory;

		public ICryptoRespository CryptoRepository { get; }
		public IProfitRepository ProfitRepository { get; }
		public ITradeRepository TradeRepository { get; }
		public ITransactionRepository TransactionRepository { get; }
		public IWalletRepository WalletRepository { get; }
		public IUserRepository UserRepository { get; }

		public ProductionUnitOfWork(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;

			CryptoRepository = new CryptoRepository(_scopeFactory);
			ProfitRepository = new ProfitRepository(_scopeFactory);
			TradeRepository = new TradeRepository(_scopeFactory);
			TransactionRepository = new TransactionRepository(_scopeFactory);
			WalletRepository = new WalletRepository(_scopeFactory);
			UserRepository = new UserRepository(_scopeFactory);
		}
		
	}
	
}

