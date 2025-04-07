﻿#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.RepositoryIntefaces
{
	public interface ITransactionRepository
	{
		Task<UserTransactionsDTO> GetUserTransactionsDTO(string userId);
		Task<TransactionDetailsDTO> GetTransactionDetailsDTO(string transactionId);
	}
}
