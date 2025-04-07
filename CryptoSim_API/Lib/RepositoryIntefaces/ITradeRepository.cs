namespace CryptoSim_API.Lib.RepositoryIntefaces
{
	public interface ITradeRepository
	{
		Task<string> BuyCrypto(TradeRequestDTO tradeRequest);
		Task<string> SellCrypto(TradeRequestDTO tradeRequest);
		Task<UserPortfolioDTO> getUserPortfolio(string userId);
	}
}
