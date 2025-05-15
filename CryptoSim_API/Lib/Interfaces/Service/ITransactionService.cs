
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Interfaces.ServiceInterfaces
{
	public interface ITransactionService
	{
		Task AddNewFeeAsync(double newFee);
		Task<Guid> CreateTransaction(NewTransactionDTO nt);
		Task DeleteUserTransactions(string userId);
		Task<TransactionDetailsDTO> GetTransactionDetailsDTO(string transactionId);
		Task<TransactionFeeReportDTO> GetUserTransactionReport(string userId);
		Task<IEnumerable<Transaction>> GetUserTransactions(string v);
		Task<IEnumerable<UserTransactionsDTO>?> GetUserTransactionsDTO(string userId);
	}
}