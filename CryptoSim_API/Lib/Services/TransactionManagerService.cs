using Microsoft.Extensions.Caching.Distributed;

namespace CryptoSim_API.Lib.Services
{
	public class TransactionManagerService
	{
		private readonly CryptoContext _dbContext;
		private readonly IDistributedCache _cache;
		public TransactionManagerService(CryptoContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
		}
		//TODOs: implement transaction manager service
		public async Task CreateTransaction(NewTransactionDTO newTransaction)
		{
			throw new NotImplementedException("TransactionManagerService.createTransaction not implemented");
		}
	}
}
