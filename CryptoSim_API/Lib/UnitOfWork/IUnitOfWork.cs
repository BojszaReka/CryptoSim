﻿using CryptoSim_API.Lib.RepositoryIntefaces;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.UnitOfWork
{
	public interface IUnitOfWork
	{
		ICryptoRespository CryptoRepository { get; }
		IProfitRepository ProfitRepository { get; }
		ITradeRepository TradeRepository { get; }
		ITransactionRepository TransactionRepository { get; }
		Task Save();
	}
}
