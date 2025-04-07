using CryptoSim_API.Lib.RepositoryIntefaces;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace CryptoSim_API.Lib.Repositories
{
	public class TransactionRepository : ITransactionRepository
	{
		private readonly CryptoContext _dbContext;
		private readonly IDistributedCache _cache;
		TransactionManagerService _transactionManager;
		public TransactionRepository(CryptoContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
			_transactionManager = new TransactionManagerService(_dbContext, _cache);
		}
		public async Task<TransactionDetailsDTO> GetTransactionDetailsDTO(string transactionId)
		{
			return await _transactionManager.GetTransactionDetailsDTO(transactionId);
		}

		public async Task<UserTransactionsDTO> GetUserTransactionsDTO(string userId)
		{
			return await _transactionManager.GetUserTransactionsDTO(userId);
		}
	}
}
