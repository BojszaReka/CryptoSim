using CryptoSim_API.Lib.Interfaces.RepositoryIntefaces;
using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using CryptoSim_Lib.Models;
using Microsoft.Extensions.Caching.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace CryptoSim_API.Lib.Repositories
{
	public class TransactionRepository : ITransactionRepository
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;
		public TransactionRepository(IServiceScopeFactory scopeFactory, CryptoContext dbContext, IMemoryCache cache)
		{
			_scopeFactory = scopeFactory;
			_dbContext = dbContext;
			_cache = cache;
		}

		private ITransactionService GetService()
		{
			var scope = _scopeFactory.CreateScope();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<ITransactionService>();
			return _cryptoManager;
		}

		public async Task<IEnumerable<UserTransactionsDTO>?> GetUserTransactionsDTO(string userId)
		{
			var _transactionManager = GetService();
			return await _transactionManager.GetUserTransactionsDTO(userId);
		}

		public async Task<TransactionDetailsDTO> GetTransactionDetailsDTO(string transactionId)
		{
			var _transactionManager = GetService();
			return await _transactionManager.GetTransactionDetailsDTO(transactionId);
		}

		public async Task<TransactionFeeReportDTO> GetUserTransactionReport(string userId)
		{
			var _transactionManager = GetService();
			return await _transactionManager.GetUserTransactionReport(userId);
		}

		public async Task<string> ChangeFeeRate(double newFee)
		{
			var _transactionManager = GetService();
			await _transactionManager.AddNewFeeAsync(newFee);
			return $"Fee rate changed successfully to: {newFee}%";
		}
	}
}
