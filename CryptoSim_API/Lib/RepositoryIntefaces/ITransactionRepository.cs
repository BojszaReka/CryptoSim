namespace CryptoSim_API.Lib.RepositoryIntefaces
{
	public interface ITransactionRepository
	{
		Task<UserTransactionsDTO> GetUserTransactionsDTO(string userId);
		Task<TransactionDetailsDTO> GetTransactionDetailsDTO(string transactionId);
	}
}
