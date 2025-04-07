using CryptoSim_Lib.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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
		private async Task<IQueryable<Transaction>> getTransactionsCache()
		{
			var cachedTransactions = await _cache.GetStringAsync("transactions");
			if (!string.IsNullOrEmpty(cachedTransactions))
			{
				var transactions = JsonConvert.DeserializeObject<List<Transaction>>(cachedTransactions);
				return transactions.AsQueryable<Transaction>();
			}
			return null;
		}

		private async Task<IQueryable<Transaction>> getTransactionsDB()
		{
			var transactionsFromDb = await _dbContext.Transactions.OrderBy(c => c.Id).ToListAsync();
			var cacheOptions = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
				SlidingExpiration = TimeSpan.FromMinutes(5)
			};
			var serializedData = JsonConvert.SerializeObject(transactionsFromDb);
			await _cache.SetStringAsync("transactions", serializedData, cacheOptions);
			return _dbContext.Transactions.OrderBy(c => c.Id).Include(t => t.User).Include(t => t.Crypto);
		}

		public async Task<IEnumerable<Transaction>> ListTransactions()
		{
			var transactions = await getTransactionsCache();
			if (transactions == null)
			{
				transactions = await getTransactionsDB();
			}
			return transactions;
		}
		//TODOs: implement transaction manager service
		public async Task<UserTransactionsDTO?> GetUserTransactionsDTO(string userId)
		{
			var transactions = await ListTransactions();
			return transactions
				.Where(t => t.UserId.Equals(userId))
				.Select(t => new UserTransactionsDTO
				{
					Type = t.Type,
					CryptoName = t.Crypto.Name,
					Quantity = t.Quantity
				}).FirstOrDefault();
		}

		public async Task<TransactionDetailsDTO> GetTransactionDetailsDTO(string transactionId)
		{
			var transactions = await ListTransactions();
			var t = transactions.Where(t => t.Id.Equals(transactionId)).FirstOrDefault();
			return new TransactionDetailsDTO {
				Type = t.Type,
				CryptoName = t.Crypto.Name,
				UserName = t.User.UserName,
				Quantity = t.Quantity,
				Price = t.Price,
				Date = t.Date
			};
		}

		public async Task CreateTransaction(NewTransactionDTO newTransaction)
		{
			var transaction = _dbContext.Database.BeginTransaction();
			Transaction t = new Transaction
			{
				Id = Guid.NewGuid(),
				UserId = newTransaction.UserId,
				CryptoId = newTransaction.CryptoId,
				Quantity = newTransaction.Quantity,
				Price = newTransaction.Price,
				Type = newTransaction.Type,
				Date = newTransaction.Date
			};
			await _dbContext.Transactions.AddAsync(t);
			await _dbContext.SaveChangesAsync();
			await _cache.RemoveAsync("transactions");
			transaction.Commit();
		}


	}
}
