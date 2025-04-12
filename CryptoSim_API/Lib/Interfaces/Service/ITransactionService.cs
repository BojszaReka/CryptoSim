
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Interfaces.ServiceInterfaces
{
	public interface ITransactionService
	{
		Task CreateTransaction(NewTransactionDTO nt);
		Task DeleteUserTransactions(string userId);
		Task<TransactionDetailsDTO> GetTransactionDetailsDTO(string transactionId);
		Task<UserTransactionsDTO> GetUserTransactionsDTO(string userId);
	}
}