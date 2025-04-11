#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Interfaces.ServiceInterfaces
{
	public interface ICryptoItemService
	{
		Task CreateCryptoItem(CryptoItem newCryptoItem);
		Task DeleteCryptoItemsByWalletId(string walletId);
		Task<IEnumerable<CryptoItem>> GetItemsWith(string v);
		Task UpdateCryptoItem(CryptoItem cryptoItem);
	}
}