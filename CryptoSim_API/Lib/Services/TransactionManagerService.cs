using CryptoSim_Lib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Services
{
	public class TransactionManagerService
	{
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;
		public TransactionManagerService(CryptoContext dbContext, IMemoryCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
		}
		private IQueryable<Transaction> getTransactionsCache()
		{
			var cachedTransactions = _cache.Get("transactions");
			if (cachedTransactions != null && !string.IsNullOrEmpty(cachedTransactions.ToString()))
			{
				var transactions = JsonConvert.DeserializeObject<List<Transaction>>(cachedTransactions.ToString());
				return transactions.AsQueryable<Transaction>();
			}
			return null;
		}

		private async Task<IQueryable<Transaction>> getTransactionsDB()
		{
			var transactionsFromDb = await _dbContext.Transactions.OrderBy(c => c.Id).ToListAsync();
			var serializedData = JsonConvert.SerializeObject(transactionsFromDb);
			_cache.Set("transactions", serializedData);
			return _dbContext.Transactions.OrderBy(c => c.Id).Include(t => t.User).Include(t => t.Crypto);
		}

		public async Task<IEnumerable<Transaction>> ListTransactions()
		{
			var transactions = getTransactionsCache();
			if (transactions == null)
			{
				transactions = await getTransactionsDB();
			}
			return transactions;
		}
		
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
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
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
				_cache.Remove("transactions");
				await transaction.CommitAsync();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error creating New Transaction:", ex);
			}
			await transaction.DisposeAsync();
		}

		internal async Task DeleteUserTransactions(string userId)
		{
			var transactions = await ListTransactions();
			var userTransactions = transactions.Where(t => t.UserId.Equals(userId));
			if (userTransactions == null)
			{
				throw new Exception("User transactions not found");
			}
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				_dbContext.Transactions.RemoveRange(userTransactions);
				await _dbContext.SaveChangesAsync();
				_cache.Remove("transactions");
				await transaction.CommitAsync();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error deleting user's Transaction:", ex);
			}
			await transaction.DisposeAsync();
		}
	}
}
