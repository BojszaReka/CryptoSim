#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using CryptoSim_Lib.Enums;

namespace CryptoSim_API.Lib.Interfaces.Service
{
	public interface IGiftService
	{
		Task<string> createGift(GiftRequestDTO request);
		Task<bool> doesGiftExists(string giftId);
		Task<Gift> getGift(string giftId);
		Task<double> GetGiftedQuantity(string v, string userId);
		Task UpdateGiftStatus(string giftId, EGiftStatus accepted);
	}
}
