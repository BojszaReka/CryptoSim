#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member


namespace CryptoSim_API.Lib.Interfaces.ServiceInterfaces
{
	public interface ITradeService
	{
		Task<Guid> BuyCrypto(TradeRequestDTO tradeRequest);
		Task<UserPortfolioDTO> getUserPortfolio(string userId);
		Task<string> giftCrypto(GiftRequestDTO request);
		Task<string?> proceddGiftCryptoAccepted(string giftId);
		Task<string?> proceddGiftCryptoRejected(string giftId);
		Task<Guid> SellCrypto(TradeRequestDTO tradeRequest);
	}
}