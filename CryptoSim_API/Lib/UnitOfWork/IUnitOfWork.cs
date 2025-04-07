using CryptoSim_API.Lib.RepositoryIntefaces;

namespace CryptoSim_API.Lib.UnitOfWork
{
	public interface IUnitOfWork
	{
		ICryptoRespository CryptoRepository { get; }
		IProfitRepository ProfitRepository { get; }
		ITradeRepository TradeRepository { get; }
		ITransactionRepository TransactionRepository { get; }
		Task Save();
	}
}
