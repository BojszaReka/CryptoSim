﻿using CryptoSim_API.Lib.Interfaces.RepositoryIntefaces;
using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Caching.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace CryptoSim_API.Lib.Repositories
{
	public class TransactionRepository : ITransactionRepository
	{
		private readonly IServiceScopeFactory _scopeFactory;
		public TransactionRepository(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
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
	}
}
