#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Interfaces.RepositoryIntefaces
{
	public interface ITradeRepository
	{
		Task<Guid> BuyCrypto(TradeRequestDTO tradeRequest);
		Task<Guid> SellCrypto(TradeRequestDTO tradeRequest);
		Task<UserPortfolioDTO> getUserPortfolio(string userId);
		Task<string> giftCrypto(GiftRequestDTO request);
		Task<string?> GiftAcceptance(string giftId, bool accepted);
	}
}
