using CryptoSim_API.Lib.Interfaces.RepositoryIntefaces;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.UnitOfWork
{
	public interface IUnitOfWork : IDisposable
	{
		ICryptoRespository CryptoRepository { get; }
		IProfitRepository ProfitRepository { get; }
		ITradeRepository TradeRepository { get; }
		ITransactionRepository TransactionRepository { get; }
		IWalletRepository WalletRepository { get; }
		IUserRepository UserRepository { get; }
		
	}
}
