#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using CryptoSim_API.Lib.Interfaces.RepositoryIntefaces;
using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CryptoSim_API.Lib.Repositories
{
	public class CryptoRepository : ICryptoRespository
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;

		public CryptoRepository(IServiceScopeFactory scopeFactory, CryptoContext dbContext, IMemoryCache cache)
		{
			_scopeFactory = scopeFactory;
			_cache = cache;
			_dbContext = dbContext;
		}

		private ICryptoService GetService()
		{
			var scope = _scopeFactory.CreateScope();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<ICryptoService>();
			return _cryptoManager;
		}

		public async Task<PriceHistoryDTO> GetPriceHistory(string cryptoId)
		{
			var _cryptoManager = GetService();
			return await _cryptoManager.GetPriceHistory(cryptoId);
		}

		public async Task<string> UpdateCryptoPrice(string cryptoId, double price)
		{
			var _cryptoManager = GetService();
			return await _cryptoManager.UpdateCryptoPrice(cryptoId, price);
		}

		public async Task<IEnumerable<CryptoDTO>> ListCryptosDTO()
		{
			var _cryptoManager = GetService();
			return await _cryptoManager.ListCryptosDTO();
		}

		public async Task<CryptoDTO> GetCryptoDTO(string Id)
		{
			var _cryptoManager = GetService();
			return await _cryptoManager.GetCryptoDTO(Id);
		}

		public async Task<string> CreateCrypto(NewCrypto newCrypto)
		{
			var _cryptoManager = GetService();
			return await _cryptoManager.CreateCrypto(newCrypto);
		}

		public async Task<string> DeleteCrypto(string Id)
		{
			var _cryptoManager = GetService();
			return await _cryptoManager.DeleteCrypto(Id);
		}
	}
}
