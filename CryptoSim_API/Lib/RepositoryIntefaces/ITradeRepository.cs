#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.RepositoryIntefaces
{
	public interface ITradeRepository
	{
		Task<string> BuyCrypto(TradeRequestDTO tradeRequest);
		Task<string> SellCrypto(TradeRequestDTO tradeRequest);
		Task<UserPortfolioDTO> getUserPortfolio(string userId);
	}
}
