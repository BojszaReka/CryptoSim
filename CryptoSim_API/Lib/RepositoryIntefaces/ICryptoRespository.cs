#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Repositories
{
	public interface ICryptoRespository
	{
		Task<PriceHistoryDTO> GetPriceHistory(string cryptoId);
		Task<string> UpdateCryptoPrice(string cryptoId, double price);
		Task<IEnumerable<CryptoDTO>> ListCryptosDTO();
		Task<CryptoDTO> GetCryptoDTO(string Id);
		Task<string> CreateCrypto(NewCrypto newCrypto);
		Task<string> DeleteCrypto(string Id);
	}
}
