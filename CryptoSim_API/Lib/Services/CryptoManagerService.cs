using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace CryptoSim_API.Lib.Services
{
	public class CryptoManagerService
	{
		private readonly CryptoContext _dbContext;
		private readonly IDistributedCache _cache;
		public CryptoManagerService(CryptoContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
		}

		private async Task<IQueryable<Crypto>> getCryptoCache()
		{
			var cachedCryptos = await _cache.GetStringAsync("cryptos");
			if (!string.IsNullOrEmpty(cachedCryptos))
			{
				var cryptos = JsonConvert.DeserializeObject<List<Crypto>>(cachedCryptos);
				return cryptos.AsQueryable<Crypto>();
			}
			return null;
		}

		private async Task<IQueryable<Crypto>> getCryptosDB()
		{
			var cryptosfFromDb = await _dbContext.Cryptos.OrderBy(c => c.Id).ToListAsync();
			var cacheOptions = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
				SlidingExpiration = TimeSpan.FromMinutes(5)
			};
			var serializedData = JsonConvert.SerializeObject(cryptosfFromDb);
			await _cache.SetStringAsync("cryptos", serializedData, cacheOptions);
			return _dbContext.Cryptos.OrderBy(c => c.Id).Include(c => c.Transactions);
		}
		//TODO: Implement crypto manager service

		private async Task<bool> doesCryptoExists(string Id)
		{
			var crypto = await GetCrypto(Id);
			return crypto != null;
		}

		private async Task<Crypto> GetCrypto(string Id)
		{
			var cryptos = await getCryptoCache();
			if (cryptos == null)
			{
				cryptos = await getCryptosDB();
			}
			var crypto = cryptos.Where(c => Id.Equals(c.Id.ToString())).FirstOrDefault();
			return crypto;
		}

		private async Task UpdateCrypto(Crypto crypto)
		{
			_dbContext.Update(crypto);
			await _dbContext.SaveChangesAsync();
			await _cache.RemoveAsync("cryptos");
		}

		//public methods:

		public async Task<String> UpdateCryptoPrice(string cryptoId, double price)
		{
			if(await doesCryptoExists(cryptoId))
			{
				Crypto c = await GetCrypto(cryptoId);
				c.PriceHistory.Add(c.CurrentPrice);
				c.CurrentPrice = price;
				await UpdateCrypto(c);
				return $"Crypto currency (id: {cryptoId}, name: {c.Name}) price updated successfully to {price}";
			}
			return "The crypto currency with the provided ID does not exist";
		}

		public async Task<PriceHistoryDTO> GetPriceHistory(string cryptoId)
		{
			if (await doesCryptoExists(cryptoId))
			{
				Crypto c = await GetCrypto(cryptoId);
				return new PriceHistoryDTO { 
					CryptoId = c.Id,
					CryptoName = c.Name,
					PriceHistory = c.PriceHistory
				};
			}
			return null;
		}	
	}
}
