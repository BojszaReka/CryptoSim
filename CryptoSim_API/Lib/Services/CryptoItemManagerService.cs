﻿#pragma warning disable CS1591 // Missing XML comment for publicly visible type or members

namespace CryptoSim_API.Lib.Services
{
	public class CryptoItemManagerService
	{
		private readonly CryptoContext _dbContext;
		private readonly IDistributedCache _cache;

		public CryptoItemManagerService(CryptoContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
		}

		public async Task<IQueryable<CryptoItem>> getCryptoItemsCache()
		{
			var cachedCryptoItems = await _cache.GetStringAsync("cryptoItems");
			if (!string.IsNullOrEmpty(cachedCryptoItems))
			{
				var cryptoItems = JsonConvert.DeserializeObject<List<CryptoItem>>(cachedCryptoItems);
				return cryptoItems.AsQueryable<CryptoItem>();
			}
			return null;
		}

		public async Task<IQueryable<CryptoItem>> getCryptoItemsDB()
		{
			var cryptoItemssfFromDb = await _dbContext.CryptoItems.OrderBy(c => c.Id).ToListAsync();
			var cacheOptions = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
				SlidingExpiration = TimeSpan.FromMinutes(5)
			};
			var serializedData = JsonConvert.SerializeObject(cryptoItemssfFromDb);
			await _cache.SetStringAsync("cryptoItems", serializedData, cacheOptions);
			return _dbContext.CryptoItems.OrderBy(c => c.Id);
		}

		public async Task CreateCryptoItem(CryptoItem newCryptoItem)
		{
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				await _dbContext.CryptoItems.AddAsync(newCryptoItem);
				await _dbContext.SaveChangesAsync();
				await _cache.RemoveAsync("cryptoItems");
				await transaction.CommitAsync();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error creating CryptoItem", ex);
			}
			transaction.Dispose();
		}

		public async Task<IQueryable<CryptoItem>> ListCryptoItems()
		{
			var cryptoItems = await getCryptoItemsCache();
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
					await _cache.RemoveAsync("cryptoItems");
					await transaction.CommitAsync();
				}
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error updating CryptoItem", ex);
			}
			transaction.Dispose();
		}

		internal async Task DeleteCryptoItemsByWalletId(string v)
		{
			throw new NotImplementedException();
		}
	}
}
