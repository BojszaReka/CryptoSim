using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using CryptoSim_Lib.Models;
using Microsoft.Extensions.Caching.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Services
{
	public class WalletManagerService : IWalletService
	{
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;

		private readonly IServiceScopeFactory _scopeFactory;

		public WalletManagerService(CryptoContext dbContext, IMemoryCache cache, IServiceScopeFactory scopeFactory) 
		{
			_dbContext = dbContext;
			_cache = cache;
			_scopeFactory = scopeFactory;
		}

		//data retrieval
		public async Task<IQueryable<Wallet>> getWalletsAsync()
		{
			var cachedWallets = _cache.Get("wallets");

			if (cachedWallets != null && !string.IsNullOrEmpty(cachedWallets.ToString()))
			{
				var wallets = JsonConvert.DeserializeObject<List<Wallet>>(cachedWallets.ToString());
				return wallets.AsQueryable<Wallet>();
			}
			var walletsfFromDb = await _dbContext.Wallets.OrderBy(c => c.Id).ToListAsync();
			var serializedData = JsonConvert.SerializeObject(walletsfFromDb);
			_cache.Set("wallets", serializedData);
			return _dbContext.Wallets.OrderBy(c => c.Id);
		}

		public async Task<IQueryable<UserWallet>> getUserWalletsAsync()
		{
			var cachedWallets = _cache.Get("userWallets");
			if (cachedWallets != null && !string.IsNullOrEmpty(cachedWallets.ToString()))
			{
				var wallets = JsonConvert.DeserializeObject<List<UserWallet>>(cachedWallets.ToString());
				return wallets.AsQueryable<UserWallet>();
			}
			var walletsfFromDb = await _dbContext.UserWallets.ToListAsync();
			var serializedData = JsonConvert.SerializeObject(walletsfFromDb);
			_cache.Set("userwallets", serializedData);
			return _dbContext.UserWallets;
		}

		//wallet lookups:
		public async Task<Wallet> GetWalletById(string walletId)
		{
			var wallets = await getWalletsAsync();
			var wallet = wallets.Where(w => walletId.Equals(w.Id.ToString())).FirstOrDefault();
			return wallet;
		}

		public async Task<Wallet> GetWalletByUserId(string userId)
		{
			var userWallets = await getUserWalletsAsync();
			var userWallet = userWallets.Where(w => userId.Equals(w.UserId.ToString())).FirstOrDefault();
			if (userWallet == null) throw new Exception("User's wallet not found");
			Guid walletid = userWallet.WalletId;
			var wallet = await GetWalletById(walletid.ToString());
			return wallet;
			
		}

		public async Task<bool> doesWalletExists(string walletId)
		{
			var wallets = await getWalletsAsync();
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
			var wallet = await GetWalletByUserId(userId);
			if (wallet == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public async Task<UserWallet> GetUserWalletByUserId(string userId)
		{
			var userWallets = await getUserWalletsAsync();
			var userwallet = userWallets.FirstOrDefault(uw => userId.Equals(uw.UserId.ToString()));
			if (userwallet == null) { throw new Exception("UserWallet not found"); }
			return userwallet;
		}

		//balance handling:
		public async Task<bool> doesUserHasBalance(string userId, double cost)
		{
			if (!await doesWalletExistsByUserId(userId)) throw new Exception("The user with the provided ID does not have a wallet");
			Wallet wallet = await GetWalletByUserId(userId);
			double balance = (double)wallet.Balance;
			if (balance < cost)
			{
				return false;
			}
			return true;
		}

		public async Task DecreaseUserBalance(string userID, double cost)
		{
			if (!await doesWalletExistsByUserId(userID)) throw new Exception("The user with the provided ID does not have a wallet");
			if (await doesUserHasBalance(userID, cost))
			{
				Wallet wallet = await GetWalletByUserId(userID);
				wallet.Balance -= cost;
				await UpdateWallet(wallet);
			}
		}

		public async Task IncreaseUserBalance(string userID, double cost)
		{
			if (!await doesWalletExistsByUserId(userID)) throw new Exception("The user with the provided ID does not have a wallet");
			if (await doesUserHasBalance(userID, cost))
			{
				Wallet wallet = await GetWalletByUserId(userID);
				wallet.Balance += cost;
				await UpdateWallet(wallet);
			}
		}
		public async Task<bool> doesUserHasCryptoBalance(string userId, string cryptoId, int quantity)
		{
			using var scope = _scopeFactory.CreateScope();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<ICryptoItemService>();

			var wallet = await GetWalletByUserId(userId);
			if (wallet == null) { scope.Dispose(); throw new Exception("The wallet with the provided ID does not exist"); }
			var cryptoItems = await _cryptoItemManager.GetItemsWith(wallet.Id.ToString());
			if (cryptoItems == null) { scope.Dispose(); throw new Exception("The user does not have any crypto currency in their wallet"); }
			var cryptoItem = cryptoItems.Where(c => c.CryptoId.ToString().Equals(cryptoId)).FirstOrDefault();
			if (cryptoItem == null) { scope.Dispose(); throw new Exception("The user does not have the crypto currency in their wallet"); }
			if (cryptoItem.Quantity >= quantity)
			{
				scope.Dispose();
				return true;
			}
			scope.Dispose();
			return false;
		}

		//wallet update, create, delete:
		public async Task UpdateWallet(Wallet wallet)
		{
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				var trackedWallet = _dbContext.Wallets.Local.FirstOrDefault(w => w.Id == wallet.Id);
				if (trackedWallet != null)
				{
					_dbContext.Entry(trackedWallet).State = EntityState.Detached;
				}

				_dbContext.Wallets.Update(wallet);
				await _dbContext.SaveChangesAsync();
				_cache.Remove("wallets");
				await transaction.CommitAsync();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception($"Error while updating Wallet: {ex}");
			}
			await transaction.DisposeAsync();
		}

		public async Task<string> DeleteWallet(string userId)
		{
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				var wallet = await GetWalletByUserId(userId);
				_dbContext.Wallets.Remove(wallet);
				await _dbContext.SaveChangesAsync();
				_cache.Remove("wallets");
				
				await transaction.CommitAsync();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error while deleting Wallet:", ex);
			}
			await transaction.DisposeAsync();
			return "Wallet deleted successfully";
		}

		public async Task<string> DeleteWalletData(string userId)
		{
			using var scope = _scopeFactory.CreateScope();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<ICryptoItemService>();

			if (!await doesWalletExistsByUserId(userId)) throw new Exception("The user with the provided ID does not have a wallet");
			var wallet = await GetWalletByUserId(userId);
			wallet.Balance = 0;
			wallet.Cryptos.Clear();
			await UpdateWallet(wallet);
			await _cryptoItemManager.DeleteCryptoItemsByWalletId(wallet.Id.ToString());
			scope.Dispose();
			return "Wallet data deleted successfully";

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

		//cryptoitem mngment

		public async Task AddCryptoToUserWallet(string userId, string cryptoID, int quantity)
		{
			using var scope = _scopeFactory.CreateScope();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<ICryptoItemService>();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<ICryptoService>();

			if (!await _cryptoManager.doesCryptoExists(cryptoID)) { scope.Dispose(); throw new Exception("The crypto currency with the provided ID does not exist"); }
			if (!await doesWalletExistsByUserId(userId)) { scope.Dispose(); throw new Exception("The user with the provided ID does not have a wallet"); }
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
				var rate = await _cryptoManager.GetCurrentRate(Guid.Parse(cryptoID));
				CryptoItem newCryptoItem = new CryptoItem
				{
					Id = Guid.NewGuid(),
					CryptoId = Guid.Parse(cryptoID),
					WalletId = wallet.Id,
					Quantity = quantity,
					BoughtAtRate = rate
				};
				await _cryptoItemManager.CreateCryptoItem(newCryptoItem);
			}
			scope.Dispose();
		}

		public async Task RemoveCryptoFromUserWallet(string userId, string cryptoID, int quantity)
		{
			using var scope = _scopeFactory.CreateScope();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<ICryptoItemService>();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<ICryptoService>();

			if (!await _cryptoManager.doesCryptoExists(cryptoID)) { scope.Dispose(); throw new Exception("The crypto currency with the provided ID does not exist"); }
			if (!await doesWalletExistsByUserId(userId)) { scope.Dispose(); throw new Exception("The user with the provided ID does not have a wallet"); }
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
			scope.Dispose();
			throw new Exception("The user does not have enough crypto currency to sell");
		}

		//dto conversions
		public async Task<List<PortfolioItem>> getUserWalletAsPortfolioList(string userId)
		{
			using var scope = _scopeFactory.CreateScope();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<ICryptoItemService>();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<ICryptoService>();

			if (!await doesWalletExistsByUserId(userId)) { scope.Dispose(); throw new Exception("The user with the provided ID does not have a wallet"); }
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
			scope.Dispose();
			return portfolio;
		}

		public async Task<WalletViewDTO> GetWalletViewDTO(string userId)
		{
			using var scope = _scopeFactory.CreateScope();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<ICryptoItemService>();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<ICryptoService>();
			var _userManager = scope.ServiceProvider.GetRequiredService<IUserService>();

			if (!await doesWalletExistsByUserId(userId)) { scope.Dispose(); throw new Exception("The user with the provided ID does not have a wallet"); }
			var wallet = await GetWalletByUserId(userId);
			var cryptoItems = await _cryptoItemManager.GetItemsWith(wallet.Id.ToString());
			WalletViewDTO walletView = new WalletViewDTO
			{
				UserName = await _userManager.getUserName(userId),
				WalletId = wallet.Id,
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
			scope.Dispose();
			return walletView;
		}

		
	}
}
