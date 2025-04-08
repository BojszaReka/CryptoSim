using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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

		public async Task<IQueryable<Crypto>> getCryptoCache()
		{
			var cachedCryptos = await _cache.GetStringAsync("cryptos");
			if (!string.IsNullOrEmpty(cachedCryptos))
			{
				var cryptos = JsonConvert.DeserializeObject<List<Crypto>>(cachedCryptos);
				return cryptos.AsQueryable<Crypto>();
			}
			return null;
		}

		public async Task<IQueryable<Crypto>> getCryptosDB()
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

		public async Task<string> UpdateCryptoPrice(string cryptoId, double price)
		{
			if (await doesCryptoExists(cryptoId))
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
				c.PriceHistory.Add(c.CurrentPrice);
				return new PriceHistoryDTO
				{
					CryptoId = c.Id,
					CryptoName = c.Name,
					PriceHistory = c.PriceHistory
				};
			}
			return null;
		}

		public async Task<IEnumerable<CryptoDTO>> ListCryptosDTO()
		{
			var cryptos = await ListCryptos();
			IEnumerable<CryptoDTO> response = cryptos.Select(c => new CryptoDTO
			{
				Id = c.Id,
				Name = c.Name,
				Rate = c.CurrentPrice
			}).ToList();
			return response;
		}

		public async Task<CryptoDTO> GetCryptoDTO(string Id)
		{
			var crypto = await GetCrypto(Id);
			if (crypto != null)
			{
				return new CryptoDTO
				{
					Id = crypto.Id,
					Name = crypto.Name,
					Rate = crypto.CurrentPrice
				};
			}
			return null;
		}

		public async Task<string> CreateCrypto(NewCrypto newCrypto)
		{
			Crypto crypto = new Crypto
			{
				Id = Guid.NewGuid(),
				Name = newCrypto.Name,
				StartingRate = newCrypto.StartingRate,
				CurrentPrice = newCrypto.StartingRate,
				PriceHistory = new List<double>(),
				Quantity = newCrypto.Quantity,
			};
			var transaction = _dbContext.Database.BeginTransaction();
			await _dbContext.Cryptos.AddAsync(crypto);
			await _dbContext.SaveChangesAsync();
			await _cache.RemoveAsync("cryptos");
			await transaction.CommitAsync();
			await transaction.DisposeAsync();
			return $"New crypto currency successfuly created with Id: {crypto.Id.ToString()} and Name: {crypto.Name}";
		}

		public async Task<string> DeleteCrypto(string Id)
		{
			if (await doesCryptoExists(Id))
			{
				var transaction = _dbContext.Database.BeginTransaction();			
				var crypto = await GetCrypto(Id);
				_dbContext.Cryptos.Remove(crypto);
				await _dbContext.SaveChangesAsync();
				await _cache.RemoveAsync("cryptos");
				await transaction.CommitAsync();
				await transaction.DisposeAsync();
				return $"Crypto currency ({crypto.Name}) successfully deleted";
			}
			return "The crypto currency with the provided ID does not exist";
		}

		public async Task<bool> doesCryptoExists(string Id)
		{
			var crypto = await GetCrypto(Id);
			if (crypto == null)
			{
				return !crypto.isDeleted;
			}
			return crypto != null;
		}

		public async Task<Crypto> GetCrypto(string Id)
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
			var transaction = _dbContext.Database.BeginTransaction();
			_dbContext.Update(crypto);
			await _dbContext.SaveChangesAsync();
			await _cache.RemoveAsync("cryptos");
			await transaction.CommitAsync();
			await transaction.DisposeAsync();
		}

		public async Task<IEnumerable<Crypto>> ListCryptos()
		{
			var cryptos = await getCryptoCache();
			if (cryptos == null)
			{
				cryptos = await getCryptosDB();
			}
			return cryptos;
		}

		public async Task<double> GetCurrentRate(Guid cryptoId)
		{
			var crypto = await GetCrypto(cryptoId.ToString());
			if (crypto != null)
			{
				return crypto.CurrentPrice;
			}
			return 0;
		}

		public async Task<string> GetCryptoName(Guid cryptoId)
		{
			var crypto = await GetCrypto(cryptoId.ToString());
			if (crypto != null)
			{
				return crypto.Name;
			}
			return null;
		}

		public async Task DecreaseCryptoQuantity(string cryptoID, int quantity)
		{
			var crypto = await GetCrypto(cryptoID);
			crypto.Quantity -= quantity;
			UpdateCrypto(crypto);
		}

		public async Task IncreaseCryptoQuantity(string cryptoID, int quantity)
		{
			var crypto = await GetCrypto(cryptoID);
			crypto.Quantity += quantity;
			UpdateCrypto(crypto);
		}
	}
}
