using CryptoSim_Lib.Models;
using Microsoft.Extensions.Caching.Distributed;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Services
{
	public class WalletManagerService
	{
		private readonly CryptoContext _dbContext;
		private readonly IDistributedCache _cache;
		CryptoItemManagerService _cryptoItemManager;
		CryptoManagerService _cryptoManager;
		UserManagerService _userManager;
		public WalletManagerService(CryptoContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
			_cryptoItemManager = new CryptoItemManagerService(_dbContext, _cache);
			_cryptoManager = new CryptoManagerService(_dbContext, _cache);
			_userManager = new UserManagerService(_dbContext, _cache);
		}
		
		public async Task<IQueryable<Wallet>> getWalletsCache()
		{
			var cachedWallets = await _cache.GetStringAsync("wallets");
			if (!string.IsNullOrEmpty(cachedWallets))
			{
				var wallets = JsonConvert.DeserializeObject<List<Wallet>>(cachedWallets);
				return wallets.AsQueryable<Wallet>().Include(w => w.Cryptos);
			}
			return null;
		}

		public async Task<IQueryable<Wallet>> getWalletsDB()
		{
			var walletsfFromDb = await _dbContext.Wallets.OrderBy(c => c.Id).ToListAsync();
			var cacheOptions = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
				SlidingExpiration = TimeSpan.FromMinutes(5)
			};
			var serializedData = JsonConvert.SerializeObject(walletsfFromDb);
			await _cache.SetStringAsync("wallets", serializedData, cacheOptions);
			return _dbContext.Wallets.OrderBy(c => c.Id).Include(c => c.Cryptos);
		}

		public async Task<IQueryable<Wallet>> ListWallets()
		{
			
			var wallets = await getWalletsCache();
			if (wallets == null)
			{
				wallets = await getWalletsDB();
			}
			return wallets;
		}

		public async Task<Wallet> GetWalletById(string walletId)
		{
			var wallets = await ListWallets();
			var wallet = wallets.Where(w => walletId.Equals(w.Id.ToString())).FirstOrDefault();
			return wallet;
		}

		public async Task<Wallet> GetWalletByUserId(string userId)
		{
			var wallets = await ListWallets();
			var wallet = wallets.Where(w => userId.Equals(w.UserId.ToString())).FirstOrDefault();
			return wallet;
		}

		public async Task<bool> doesWalletExists(string walletId)
		{
			var wallets = await ListWallets();
			var wallet = wallets.Where(w => walletId.Equals(w.Id.ToString())).FirstOrDefault();
			if (wallet == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public async Task<bool> doesWalletExistsByUserId(string userId)
		{
			var wallets = await ListWallets();
			var wallet = wallets.Where(w => userId.Equals(w.UserId.ToString())).FirstOrDefault();
			if (wallet == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public async Task<IEnumerable<CryptoItem>> getCryptoItems(string walletId)
		{
			if(await doesWalletExists(walletId))
			{
				var cryptoItems = await _cryptoItemManager.GetItemsWith(walletId);
				List<CryptoItem> cryptoItemsList = cryptoItems.ToList();
				foreach (var cryptoItem in cryptoItems)
				{
					var crypto = await _cryptoManager.GetCrypto(cryptoItem.CryptoId.ToString());
					if(!crypto.isDeleted)
					{
						cryptoItemsList.Add(cryptoItem);
					}

				}
				return cryptoItemsList.AsEnumerable();
			}
			return null;
		}

		public async Task<bool> doesUserHasBalance(string userId, double cost)
		{
			if (!await doesWalletExistsByUserId(userId)) throw new Exception("The user with the provided ID does not have a wallet");
			Wallet wallet = await GetWalletByUserId(userId);
			double balance = (double)wallet.Balance;
			if (balance < cost)
			{
				return false;
			}
			return false;
		}

		public async Task DecreaseUserBalance(string userID, double cost)
		{
			if (!await doesWalletExistsByUserId(userID)) throw new Exception("The user with the provided ID does not have a wallet");
			if (await doesUserHasBalance(userID, cost))
			{
				Wallet wallet = await GetWalletByUserId(userID);
				wallet.Balance -= (decimal) cost;
				await UpdateWallet(wallet);
			}
		}

		public async Task IncreaseUserBalance(string userID, double cost)
		{
			if (!await doesWalletExistsByUserId(userID)) throw new Exception("The user with the provided ID does not have a wallet");
			if (await doesUserHasBalance(userID, cost))
			{
				Wallet wallet = await GetWalletByUserId(userID);
				wallet.Balance += (decimal)cost;
				await UpdateWallet(wallet);
			}
		}

		public async Task UpdateWallet(Wallet wallet)
		{
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			_dbContext.Wallets.Update(wallet);
			await _dbContext.SaveChangesAsync();
			await _cache.RemoveAsync("wallets");
			await transaction.CommitAsync();
			await transaction.DisposeAsync();
		}

		public async Task AddCryptoToUserWallet(string userId, string cryptoID, int quantity)
		{
			if(!await _cryptoManager.doesCryptoExists(cryptoID)) throw new Exception("The crypto currency with the provided ID does not exist");
			if(!await doesWalletExistsByUserId(userId)) throw new Exception("The user with the provided ID does not have a wallet");
			var wallet = await GetWalletByUserId(userId);
			var cryptoItems = await _cryptoItemManager.GetItemsWith(wallet.Id.ToString());
			var cryptoItem = cryptoItems.Where(c => c.CryptoId.Equals(cryptoID)).FirstOrDefault();
			if (cryptoItem != null)
			{
				cryptoItem.Quantity += quantity;
				await _cryptoItemManager.UpdateCryptoItem(cryptoItem);
			}
			else
			{
				CryptoItem newCryptoItem = new CryptoItem
				{
					Id = Guid.NewGuid(),
					CryptoId = Guid.Parse(cryptoID),
					WalletId = wallet.Id,
					Quantity = quantity
				};
				await _cryptoItemManager.CreateCryptoItem(newCryptoItem);
			}
		}

		public async Task RemoveCryptoFromUserWallet(string userId, string cryptoID, int quantity)
		{
			if (!await _cryptoManager.doesCryptoExists(cryptoID)) throw new Exception("The crypto currency with the provided ID does not exist");
			if (!await doesWalletExistsByUserId(userId)) throw new Exception("The user with the provided ID does not have a wallet");
			var wallet = await GetWalletByUserId(userId);
			var cryptoItems = await _cryptoItemManager.GetItemsWith(wallet.Id.ToString());
			var cryptoItem = cryptoItems.Where(c => c.CryptoId.Equals(cryptoID)).FirstOrDefault();
			if (cryptoItem != null)
			{
				if (cryptoItem.Quantity >= quantity)
				{
					cryptoItem.Quantity -= quantity;
					await _cryptoItemManager.UpdateCryptoItem(cryptoItem);
				}
			}
			throw new Exception("The user does not have enough crypto currency to sell");
		}

		public async Task<List<PortfolioItem>> getUserWalletAsPortfolioList(string userId)
		{
			if (!await doesWalletExistsByUserId(userId)) throw new Exception("The user with the provided ID does not have a wallet");
			var wallet = await GetWalletByUserId(userId);
			var cryptoItems = await _cryptoItemManager.GetItemsWith(wallet.Id.ToString());
			var portfolio = new List<PortfolioItem>();
			foreach ( var cryptoItem in cryptoItems)
			{
				var crypto = await _cryptoManager.GetCrypto(cryptoItem.CryptoId.ToString());
				portfolio.Add(new PortfolioItem
				{
					CryptoName = crypto.Name,
					Quantity = cryptoItem.Quantity,
					CurrentValue = crypto.CurrentPrice * cryptoItem.Quantity
				});
				
			}
			return portfolio;
		}

		public async Task<string> DeleteWalletData(string userId)
		{
			if (!await doesWalletExistsByUserId(userId)) throw new Exception("The user with the provided ID does not have a wallet");
			var wallet = await GetWalletByUserId(userId);
			wallet.Balance = 0;
			wallet.Cryptos.Clear();
			await UpdateWallet(wallet);
			await _cryptoItemManager.DeleteCryptoItemsByWalletId(wallet.Id.ToString());
			return "Wallet data deleted successfully";

		}

		public async Task<string> DeleteWallet(string userId)
		{
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			var wallet = await GetWalletByUserId(userId);
			_dbContext.Wallets.Remove(wallet);
			await _dbContext.SaveChangesAsync();
			await _cache.RemoveAsync("wallets");
			await transaction.CommitAsync();
			await transaction.DisposeAsync();
			await _cryptoItemManager.DeleteCryptoItemsByWalletId(wallet.Id.ToString());
			return "Wallet deleted successfully";
		}

		public async Task<string> UpdateWallet(WalletUpdateDTO walletUpdate) //userid + balance
		{
			if (!await doesWalletExistsByUserId(walletUpdate.UserId)) throw new Exception("The user with the provided ID does not have a wallet");
			var wallet = await GetWalletByUserId(walletUpdate.UserId);
			if (wallet == null) throw new Exception("The wallet with the provided ID does not exist");
			wallet.Balance = walletUpdate.Balance;
			await UpdateWallet(wallet);
			return $"Wallet updated successfully, new balance: {wallet.Balance}";
		}

		internal async Task<WalletViewDTO> GetWalletViewDTO(string userId)
		{
			if (!await doesWalletExistsByUserId(userId)) throw new Exception("The user with the provided ID does not have a wallet");
			var wallet = await GetWalletByUserId(userId);
			if (wallet == null) throw new Exception("The wallet with the provided ID does not exist");
			var cryptoItems = await _cryptoItemManager.GetItemsWith(wallet.Id.ToString());
			WalletViewDTO walletView = new WalletViewDTO
			{
				UserName = await _userManager.getUserName(userId),
				Balance = wallet.Balance,
				Cryptos = new List<string>()
			};
			foreach (var cryptoItem in cryptoItems)
			{
				var crypto = await _cryptoManager.GetCrypto(cryptoItem.CryptoId.ToString());
				if (!crypto.isDeleted)
				{
					walletView.Cryptos.Add(crypto.Name);
				}
			}
			return walletView;
		}

		internal async Task<bool> doesUserHasCryptoBalance(string userId, string cryptoId, int quantity)
		{
			var wallet = await GetWalletByUserId(userId);
			if (wallet == null) throw new Exception("The wallet with the provided ID does not exist");
			var cryptoItems = await _cryptoItemManager.GetItemsWith(wallet.Id.ToString());
			var cryptoItem = cryptoItems.Where(c => c.CryptoId.Equals(cryptoId)).FirstOrDefault();
			if (cryptoItem == null) throw new Exception("The user does not have the crypto currency in their wallet");
			if (cryptoItem.Quantity >= quantity)
			{
				return true;
			}
			return false;
		}
	}
}
