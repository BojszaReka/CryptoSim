using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Services
{
	public class CryptoManagerService : ICryptoService
	{
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;
		public CryptoManagerService(CryptoContext dbContext, IMemoryCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
		}

		public IQueryable<Crypto> getCryptoCache()
		{
			var cachedCryptos = _cache.Get("cryptos");
			if (cachedCryptos != null && !string.IsNullOrEmpty(cachedCryptos.ToString()))
			{
				var cryptos = JsonConvert.DeserializeObject<List<Crypto>>(cachedCryptos.ToString());
				return cryptos.AsQueryable<Crypto>();
			}
			return null;
		}

		public async Task<IQueryable<Crypto>> getCryptosDB()
		{
			var cryptosfFromDb = await _dbContext.Cryptos.OrderBy(c => c.Id).ToListAsync();
			var serializedData = JsonConvert.SerializeObject(cryptosfFromDb);
			_cache.Set("cryptos", serializedData);
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
			throw new Exception("The crypto currency with the provided ID does not exist");
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
			if (!await doesCryptoExists(Id)) throw new Exception("The crypto with the ID doesnt exists");
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
			//A rendszer 15 féle kriptovalutát kezel maximum
			var cryptos = await ListCryptos();
			if (cryptos.Count() >= 15)
			{
				throw new Exception("The maximum number of crypto currencies is 15");
			}

			Crypto crypto = new Crypto
			{
				Id = Guid.NewGuid(),
				Name = newCrypto.Name,
				StartingRate = newCrypto.StartingRate,
				CurrentPrice = newCrypto.StartingRate,
				PriceHistory = new List<double>(),
				Quantity = newCrypto.Quantity,
			};

			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				await _dbContext.Cryptos.AddAsync(crypto);
				await _dbContext.SaveChangesAsync();
				_cache.Remove("cryptos");
				await transaction.CommitAsync();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error creating Crypto Currency", ex);
			}
			await transaction.DisposeAsync();
			return $"New crypto currency successfuly created with Id: {crypto.Id.ToString()} and Name: {crypto.Name}";

		}

		public async Task<string> DeleteCrypto(string Id)
		{
			if (await doesCryptoExists(Id))
			{
				var crypto = await GetCrypto(Id);

				if (crypto != null)
				{
					var transaction = await _dbContext.Database.BeginTransactionAsync();
					try
					{
						_dbContext.Cryptos.Remove(crypto);
						await _dbContext.SaveChangesAsync();
						_cache.Remove("cryptos");
						await transaction.CommitAsync();
					}
					catch (Exception ex)
					{
						await transaction.RollbackAsync();
						throw new Exception("Error deleting Crypto Currency:", ex);
					}
					await transaction.DisposeAsync();
					return $"Crypto currency ({crypto.Name}) successfully deleted";
				}
				else
				{
					throw new Exception("The crypto currency is null");
				}
			}
			else
			{
				throw new Exception($"The crypto currency with the provided ID ({Id}) does not exist");
			}
			
		}

		public async Task<bool> doesCryptoExists(string Id)
		{
			var crypto = await GetCrypto(Id);
			if (crypto == null)
			{
				return false;
			}
			return !crypto.isDeleted;
		}

		public async Task<Crypto> GetCrypto(string cryptoId)
		{
			var cryptos = await ListCryptos();
			Crypto crypto = cryptos.FirstOrDefault(c => cryptoId.Equals(c.Id.ToString()));
			return crypto;
		}

		public async Task UpdateCrypto(Crypto crypto)
		{
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				_dbContext.Update(crypto);
				await _dbContext.SaveChangesAsync();
				_cache.Remove("cryptos");
				await transaction.CommitAsync();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error updating Crypto currency", ex);
			}			
			await transaction.DisposeAsync();
		}

		public async Task<IEnumerable<Crypto>> ListCryptos()
		{
			var cryptos = getCryptoCache();
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
			await UpdateCrypto(crypto);
		}

		public async Task IncreaseCryptoQuantity(string cryptoID, int quantity)
		{
			var crypto = await GetCrypto(cryptoID);
			crypto.Quantity += quantity;
			await UpdateCrypto(crypto);
		}
	}
}
