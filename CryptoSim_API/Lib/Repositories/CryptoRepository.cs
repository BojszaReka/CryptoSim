#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Repositories
{
	public class CryptoRepository : ICryptoRespository
	{
		private readonly CryptoContext _dbContext;
		private readonly IDistributedCache _cache;
		CryptoManagerService _cryptoManager;
		public CryptoRepository(CryptoContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
			_cryptoManager = new CryptoManagerService(_dbContext, _cache);
		}

		public async Task<PriceHistoryDTO> GetPriceHistory(string cryptoId)
		{
			return await _cryptoManager.GetPriceHistory(cryptoId);
		}

		public async Task<string> UpdateCryptoPrice(string cryptoId, double price)
		{
			return await _cryptoManager.UpdateCryptoPrice(cryptoId, price);
		}

		public async Task<IEnumerable<CryptoDTO>> ListCryptosDTO()
		{
			return await _cryptoManager.ListCryptosDTO();
		}

		public async Task<CryptoDTO> GetCryptoDTO(string Id)
		{
			return await _cryptoManager.GetCryptoDTO(Id);
		}

		public async Task<string> CreateCrypto(NewCrypto newCrypto)
		{
			return await _cryptoManager.CreateCrypto(newCrypto);
		}

		public async Task<string> DeleteCrypto(string Id)
		{
			return await _cryptoManager.DeleteCrypto(Id);
		}
	}
}
