﻿using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
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
		
		public IQueryable<Wallet> getWalletsCache()
		{
			var cachedWallets = _cache.Get("wallets");
			
			if (cachedWallets != null && !string.IsNullOrEmpty(cachedWallets.ToString()))
			{
				var wallets = JsonConvert.DeserializeObject<List<Wallet>>(cachedWallets.ToString());
				return wallets.AsQueryable<Wallet>().Include(w => w.Cryptos);
			}
			return null;
		}

		public IQueryable<UserWallet> getUserWalletsCache()
		{
			var cachedWallets = _cache.Get("userWallets");

			if (cachedWallets != null && !string.IsNullOrEmpty(cachedWallets.ToString()))
			{
				var wallets = JsonConvert.DeserializeObject<List<UserWallet>>(cachedWallets.ToString());
				return wallets.AsQueryable<UserWallet>();
			}
			return null;
		}

		public async Task<IQueryable<Wallet>> getWalletsDB()
		{
			var walletsfFromDb = await _dbContext.Wallets.OrderBy(c => c.Id).ToListAsync();
			var serializedData = JsonConvert.SerializeObject(walletsfFromDb);
			_cache.Set("wallets", serializedData);
			return _dbContext.Wallets.OrderBy(c => c.Id).Include(c => c.Cryptos);
		}

		public async Task<IQueryable<UserWallet>> getUserWalletsDB()
		{
			var walletsfFromDb = await _dbContext.UserWallets.ToListAsync();
			var serializedData = JsonConvert.SerializeObject(walletsfFromDb);
			_cache.Set("wallets", serializedData);
			return _dbContext.UserWallets;
		}

		public async Task<IQueryable<UserWallet>> ListUserWallets()
		{
			var userwallets = getUserWalletsCache();
			if (userwallets == null)
			{
				userwallets = await getUserWalletsDB();
			}
			return userwallets;
		}

		public async Task<IQueryable<Wallet>> ListWallets()
		{
			
			var wallets = getWalletsCache();
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
			var userWallets = await ListUserWallets();
			var userWallet = userWallets.Where(w => userId.Equals(w.UserId.ToString())).FirstOrDefault();

			var wallets = await ListWallets();
			var wallet = wallets.Where(w => w.Id.Equals(userWallet.WalletId)).FirstOrDefault();
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
			var wallet = GetWalletByUserId(userId);
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
			using var scope = _scopeFactory.CreateScope();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<CryptoItemManagerService>();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<CryptoManagerService>();

			if (await doesWalletExists(walletId))
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
			return true;
		}

		public async Task DecreaseUserBalance(string userID, double cost)
		{
			if (!await doesWalletExistsByUserId(userID)) throw new Exception("The user with the provided ID does not have a wallet");
			if (await doesUserHasBalance(userID, cost))
			{
				Wallet wallet = await GetWalletByUserId(userID);
				wallet.Balance -=  cost;
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

		public async Task UpdateWallet(Wallet wallet)
		{
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				var existingWallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == wallet.Id);

				if (existingWallet == null) throw new Exception("Wallet not found.");

				_dbContext.Entry(existingWallet).CurrentValues.SetValues(wallet);

				//_dbContext.Wallets.Update(wallet);
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

		public async Task AddCryptoToUserWallet(string userId, string cryptoID, int quantity)
		{
			using var scope = _scopeFactory.CreateScope();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<CryptoItemManagerService>();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<CryptoManagerService>();

			if (!await _cryptoManager.doesCryptoExists(cryptoID)) throw new Exception("The crypto currency with the provided ID does not exist");
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
			using var scope = _scopeFactory.CreateScope();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<CryptoItemManagerService>();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<CryptoManagerService>();

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
			using var scope = _scopeFactory.CreateScope();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<CryptoItemManagerService>();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<CryptoManagerService>();

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
			using var scope = _scopeFactory.CreateScope();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<CryptoItemManagerService>();

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
			using var scope = _scopeFactory.CreateScope();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<CryptoItemManagerService>();

			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				var wallet = await GetWalletByUserId(userId);
				_dbContext.Wallets.Remove(wallet);
				await _dbContext.SaveChangesAsync();
				_cache.Remove("wallets");
				await _cryptoItemManager.DeleteCryptoItemsByWalletId(wallet.Id.ToString());
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

		public async Task<string> UpdateWallet(WalletUpdateDTO walletUpdate) //userid + balance
		{
			if (!await doesWalletExistsByUserId(walletUpdate.UserId)) throw new Exception("The user with the provided ID does not have a wallet");
			var wallet = await GetWalletByUserId(walletUpdate.UserId);
			if (wallet == null) throw new Exception("The wallet with the provided ID does not exist");
			wallet.Balance = walletUpdate.Balance;
			await UpdateWallet(wallet);
			return $"Wallet updated successfully, new balance: {wallet.Balance}";
		}

		public async Task<WalletViewDTO> GetWalletViewDTO(string userId)
		{
			using var scope = _scopeFactory.CreateScope();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<CryptoItemManagerService>();
			var _cryptoManager = scope.ServiceProvider.GetRequiredService<CryptoManagerService>();
			var _userManager = scope.ServiceProvider.GetRequiredService<UserManagerService>();

			if (!await doesWalletExistsByUserId(userId)) throw new Exception("The user with the provided ID does not have a wallet");
			var wallet = await GetWalletByUserId(userId);
			if (wallet == null) throw new Exception("The wallet with the provided ID does not exist");
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
			return walletView;
		}

		public async Task<bool> doesUserHasCryptoBalance(string userId, string cryptoId, int quantity)
		{
			using var scope = _scopeFactory.CreateScope();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<CryptoItemManagerService>();

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

		public async Task<Guid> CreateUserWallet(Guid userId)
		{
			Wallet wallet = new Wallet
			{
				Id = Guid.NewGuid(),
				Balance = 10000
			};

			UserWallet userWallet = new UserWallet
			{
				UserId = userId,
				WalletId = wallet.Id
			};

			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				await _dbContext.Wallets.AddAsync(wallet);
				await _dbContext.SaveChangesAsync();
				await _dbContext.UserWallets.AddAsync(userWallet);
				await _dbContext.SaveChangesAsync();
				_cache.Remove("userWallets");
				_cache.Remove("wallets");
				await transaction.CommitAsync();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error creating user's Wallet:", ex);
			}
			await transaction.DisposeAsync();
			return wallet.Id;
		}
	}
}
