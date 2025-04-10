#pragma warning disable CS1591 // Missing XML comment for publicly visible type or members

using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CryptoSim_API.Lib.Services
{
	public class CryptoItemManagerService : ICryptoItemService
	{
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;

		public CryptoItemManagerService(CryptoContext dbContext, IMemoryCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
		}

		public IQueryable<CryptoItem> getCryptoItemsCache()
		{
			var cachedCryptoItems = _cache.Get("cryptoItems");
			if (cachedCryptoItems != null && !string.IsNullOrEmpty(cachedCryptoItems.ToString()))
			{
				var cryptoItems = JsonConvert.DeserializeObject<List<CryptoItem>>(cachedCryptoItems.ToString());
				return cryptoItems.AsQueryable<CryptoItem>();
			}
			return null;
		}

		public async Task<IQueryable<CryptoItem>> getCryptoItemsDB()
		{
			var cryptoItemssfFromDb = await _dbContext.CryptoItems.OrderBy(c => c.Id).ToListAsync();
			var serializedData = JsonConvert.SerializeObject(cryptoItemssfFromDb);
			_cache.Set("cryptoItems", serializedData);
			return _dbContext.CryptoItems.OrderBy(c => c.Id);
		}

		public async Task CreateCryptoItem(CryptoItem newCryptoItem)
		{
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				await _dbContext.CryptoItems.AddAsync(newCryptoItem);
				await _dbContext.SaveChangesAsync();
				_cache.Remove("cryptoItems");
				await transaction.CommitAsync();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error creating CryptoItem", ex);
			}
			await transaction.DisposeAsync();
		}

		public async Task<IQueryable<CryptoItem>> ListCryptoItems()
		{
			var cryptoItems = getCryptoItemsCache();
			if (cryptoItems == null)
			{
				cryptoItems = await getCryptoItemsDB();
			}
			return cryptoItems;
		}

		internal async Task<IEnumerable<CryptoItem>> GetItemsWith(string walletId)
		{
			var cryptoItems = await ListCryptoItems();
			var filteredItems = cryptoItems.Where(c => c.WalletId.ToString() == walletId);
			return filteredItems.OrderBy(c => c.Id).ToList();
		}

		internal async Task UpdateCryptoItem(CryptoItem cryptoItem)
		{
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				var existingCryptoItem = await _dbContext.CryptoItems.FindAsync(cryptoItem.Id);
				if (existingCryptoItem != null)
				{
					existingCryptoItem.Quantity = cryptoItem.Quantity;
					existingCryptoItem.BoughtAtRate = cryptoItem.BoughtAtRate;
					await _dbContext.SaveChangesAsync();
					_cache.Remove("cryptoItems");
					await transaction.CommitAsync();
					
				}
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error updating CryptoItem", ex);
			}
			await transaction.DisposeAsync();
		}

		internal async Task DeleteCryptoItemsByWalletId(string walletId)
		{
			var items = await GetItemsWith(walletId);
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				foreach (var item in items)
				{
					var existingCryptoItem = await _dbContext.CryptoItems.FindAsync(item.Id);
					if (existingCryptoItem != null)
					{
						_dbContext.CryptoItems.Remove(existingCryptoItem);
					}
				}
				await _dbContext.SaveChangesAsync();
				_cache.Remove("cryptoItems");
				await transaction.CommitAsync();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error deleting CryptoItem", ex);
			}
			await transaction.DisposeAsync();
		}
	}
}
