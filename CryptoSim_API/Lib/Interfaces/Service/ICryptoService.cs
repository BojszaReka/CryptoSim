
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member




namespace CryptoSim_API.Lib.Interfaces.ServiceInterfaces
{
	public interface ICryptoService
	{
		Task<string> CreateCrypto(NewCrypto newCrypto);
		Task DecreaseCryptoQuantity(string v, int quantity);
		Task<string> DeleteCrypto(string id);
		Task<bool> doesCryptoExists(string v);
		Task<Crypto> GetCrypto(string v);
		Task<CryptoDTO> GetCryptoDTO(string id);
		Task<string> GetCryptoName(Guid id);
		Task<double> GetCurrentRate(Guid cryptoId);
		Task<PriceHistoryDTO> GetPriceHistory(string cryptoId);
		Task IncreaseCryptoQuantity(string v, int quantity);
		Task<bool> isEnoughCrypto(string cryptoId, int quantity);
		Task<IEnumerable<Crypto>> ListCryptos();
		Task<IEnumerable<CryptoDTO>> ListCryptosDTO();
		Task RandomBulkUpgrade();
		Task<string> UpdateCryptoPrice(string cryptoId, double price);
	}
}
