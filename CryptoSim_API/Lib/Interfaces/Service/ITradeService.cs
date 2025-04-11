#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member


namespace CryptoSim_API.Lib.Interfaces.ServiceInterfaces
{
	public interface ITradeService
	{
		Task<string> BuyCrypto(TradeRequestDTO tradeRequest);
		Task<UserPortfolioDTO> getUserPortfolio(string userId);
		Task<string> SellCrypto(TradeRequestDTO tradeRequest);
	}
}