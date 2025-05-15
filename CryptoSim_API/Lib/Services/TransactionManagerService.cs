using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using CryptoSim_Lib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Services
{
	public class TransactionManagerService : ITransactionService
	{
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;
		private readonly IServiceScopeFactory _scopeFactory;

		public TransactionManagerService(CryptoContext dbContext, IMemoryCache cache, IServiceScopeFactory scopeFactory)
		{
			_dbContext = dbContext;
			_cache = cache;
			_scopeFactory = scopeFactory;
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
		
		public async Task<IEnumerable<UserTransactionsDTO>?> GetUserTransactionsDTO(string userId)
		{
			var transactions = await ListTransactions();
			var filtered = transactions.Where( t => userId.Equals(t.UserId.ToString())).ToList();
			if(filtered == null)
			{
				throw new Exception("The user does not have any transactions");
			}

			var scope = _scopeFactory.CreateScope();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<ICryptoService>();
			foreach (var transaction in filtered) { 
				transaction.Crypto = await _cryptoManager.GetCrypto(transaction.CryptoId.ToString());
			}
			scope.Dispose();

			return filtered
				.Select(t => new UserTransactionsDTO
				{
					TransactionId = t.Id,
					Type = t.Type.ToString(),
					CryptoName = t.Crypto.Name,
					Quantity = t.Quantity
				});
		}

		public async Task<TransactionDetailsDTO> GetTransactionDetailsDTO(string transactionId)
		{
			var transactions = await ListTransactions();

			//todo: ellenorizni h letezik-e a tranzakcio
			
			var scope = _scopeFactory.CreateScope();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<ICryptoService>();
			var _userManager = scope.ServiceProvider.GetRequiredService<IUserService>();

			foreach (var transaction in transactions) {
				transaction.Crypto = await _cryptoManager.GetCrypto(transaction.CryptoId.ToString());
				transaction.User = await _userManager.getUser(transaction.UserId.ToString());
			}
			scope.Dispose();

			var t = transactions.Where(t => t.Id.ToString().Equals(transactionId)).FirstOrDefault();
			if (t == null)
			{
				throw new Exception("Transaction not found");
			}

			double _feePercentage = await getLatestFee();

			if (t.Fee == null)
			{
				t.Fee = t.Price * _feePercentage;
			}

			return new TransactionDetailsDTO {
				Type = t.Type.ToString(),
				CryptoName = t.Crypto.Name, //TODO: nulll reference!
				UserName = t.User.UserName,
				Quantity = t.Quantity,
				Price = t.Price,
				Date = t.Date,
				Fee = t.Fee
			};
		}

		public async Task<Guid> CreateTransaction(NewTransactionDTO newTransaction)
		{
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				double _feePercentage = await getLatestFee();

				Transaction t = new Transaction
				{
					Id = Guid.NewGuid(),
					UserId = newTransaction.UserId,
					CryptoId = newTransaction.CryptoId,
					Quantity = newTransaction.Quantity,
					Price = newTransaction.Price,
					Type = newTransaction.Type,
					Date = newTransaction.Date,
					Fee = newTransaction.Price * _feePercentage
				};
				await _dbContext.Transactions.AddAsync(t);
				await _dbContext.SaveChangesAsync();
				_cache.Remove("transactions");
				await transaction.CommitAsync();
				return t.Id;
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error creating New Transaction:", ex);
			}
			finally
			{
				await transaction.DisposeAsync();
			}
		}

		public async Task DeleteUserTransactions(string userId)
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

		public async Task<IEnumerable<Transaction>> GetUserTransactions(string userId)
		{
			var transactions = await ListTransactions();
			var usertrans = transactions.Where(t => userId.Equals(t.UserId.ToString())).ToList();
			return usertrans;
		}

		public async Task<TransactionFeeReportDTO> GetUserTransactionReport(string userId)
		{
			if (!Guid.TryParse(userId, out Guid userGuid))
			{
				throw new ArgumentException("Invalid userId");
			}

			var userTransactions = await GetUserTransactions(userId);

			double _feePercentage = await getLatestFee();

			foreach (var transaction in userTransactions)
			{
				if (transaction.Fee == 0)
				{
					transaction.Fee = transaction.Price * _feePercentage;
				}
			}

			double totalFee = userTransactions.Sum(t => t.Fee);

			var dailyFees = userTransactions
				.GroupBy(t => t.Date.Date)
				.Select(g => new DailyFeeSummaryDTO
				{
					Date = g.Key,
					TotalFee = g.Sum(t => t.Fee)
				})
				.OrderBy(d => d.Date)
				.ToList();

			var transactionDetails = userTransactions
				.Select(t => new TransactionFeeDetailDTO
				{
					TransactionId = t.Id,
					UserId = t.UserId,
					CryptoId = t.CryptoId,
					Fee = t.Fee,
					Timestamp = t.Date
				})
				.ToList();

			var report = new TransactionFeeReportDTO
			{
				TotalFee = totalFee,
				DailyFees = dailyFees,
				TransactionDetails = transactionDetails
			};

			return report;
		}

		public async Task<double> getLatestFee()
		{
			var fees = await _dbContext.Fees.ToListAsync();

			double _feePercentage;

			if (fees == null || !fees.Any())
			{
				// Nincs egyetlen fee sem, hozzáadunk egy alapértelmezettet (0.2%)
				var defaultFee = new Fee
				{
					Percentage = 0.002
				};

				_dbContext.Fees.Add(defaultFee);
				await _dbContext.SaveChangesAsync();

				_feePercentage = defaultFee.Percentage;
			}
			else
			{
				// Legutolsó (legfrissebb) fee-t használjuk
				var latestFee = fees.OrderByDescending(f => f.CreatedAt).First();
				_feePercentage = latestFee.Percentage;
			}

			return _feePercentage;
		}

		public async Task AddNewFeeAsync(double percentage)
		{
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				if (percentage < 0 || percentage > 50)
				{
					throw new ArgumentOutOfRangeException(nameof(percentage), "Fee percentage must be between 0 and 50.");
				}

				var fee = new Fee
				{
					Percentage = percentage / 100.0  // százalékban kapod, pl. 0.2 → 0.002
				};

				_dbContext.Fees.Add(fee);
				await _dbContext.SaveChangesAsync();
				await _dbContext.Fees.AddAsync(fee);
				await _dbContext.SaveChangesAsync();
				_cache.Remove("transactions");
				await transaction.CommitAsync();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error adding new fee:", ex);
			}
			finally
			{
				await transaction.DisposeAsync();
			}
		}

	}
}
