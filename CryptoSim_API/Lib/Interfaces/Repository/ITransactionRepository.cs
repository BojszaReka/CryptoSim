#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Interfaces.RepositoryIntefaces
{
	public interface ITransactionRepository
	{
		Task<IEnumerable<UserTransactionsDTO>?> GetUserTransactionsDTO(string userId);
		Task<TransactionDetailsDTO> GetTransactionDetailsDTO(string transactionId);
		Task<TransactionFeeReportDTO> GetUserTransactionReport(string userId);
		Task<string> ChangeFeeRate(double newFee);
	}
}
