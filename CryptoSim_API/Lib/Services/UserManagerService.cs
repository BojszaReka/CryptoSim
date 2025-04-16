using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using CryptoSim_Lib.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Transactions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Services
{
	public class UserManagerService : IUserService
	{
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;

		private readonly IServiceScopeFactory _scopeFactory;

		public UserManagerService(CryptoContext dbContext, IMemoryCache cache, IServiceScopeFactory scopeFactory)
		{
			_dbContext = dbContext;
			_cache = cache;
			_scopeFactory = scopeFactory;
		}


		public IQueryable<User> getUsersCache()
		{
			var cachedUsers = _cache.Get("users");
			if (cachedUsers != null && !string.IsNullOrEmpty(cachedUsers.ToString()))
			{
				var users = JsonConvert.DeserializeObject<List<User>>(cachedUsers.ToString());
				return users.AsQueryable<User>();
			}
			return null;
		}

		public async Task<IQueryable<User>> getUsersDB()
		{
			var usersFromDb = await _dbContext.Users.OrderBy(c => c.Id).ToListAsync();
			var serializedData = JsonConvert.SerializeObject(usersFromDb);
			_cache.Set("users", serializedData);
			return usersFromDb.AsQueryable<User>();
		}
		public async Task<IQueryable<User>> ListUsers()
		{
			var users = getUsersCache();
			if (users == null)
			{
				users = await getUsersDB();
			}
			return users;
		}

		public async Task<User> getUser(string Id)
		{
			var users = await ListUsers();
			var user = users.FirstOrDefault(u => u.Id.ToString() == Id);
			return user;
		}

		public async Task<string> getUserName(string Id)
		{
			if(await doesUserExists(Id))
			{
				User u = await getUser(Id);
				return u.UserName;
			}
			throw new Exception("User not found");
		}

		public async Task<bool> doesUserExists(string Id)
		{
			var user = await getUser(Id);
			if (user != null)
			{
				return true;
			}
			return false;
		}

		public async Task<string> DeleteUser(string userId)
		{
			using var scope = _scopeFactory.CreateScope();
			var _walletManager = scope.ServiceProvider.GetRequiredService<IWalletService>();
			var _transactionManager = scope.ServiceProvider.GetRequiredService<ITransactionService>();
			var _cryptoItemManager = scope.ServiceProvider.GetRequiredService<ICryptoItemService>();


			if (await doesUserExists(userId))
			{
				var transaction = await _dbContext.Database.BeginTransactionAsync();
				try
				{
					User u = await getUser(userId);
					Wallet wallet = await _walletManager.GetWalletByUserId(u.Id.ToString());
					UserWallet uw = await _walletManager.GetUserWalletByUserId(u.Id.ToString());
					var transactions = await _transactionManager.GetUserTransactions(u.Id.ToString());
					var cryptoitems = await _cryptoItemManager.GetItemsWith(wallet.Id.ToString());

					_dbContext.Transactions.RemoveRange(transactions);
					_dbContext.CryptoItems.RemoveRange(cryptoitems);
					_dbContext.UserWallets.Remove(uw);
					_dbContext.Users.Remove(u);
					_dbContext.Wallets.Remove(wallet);
					
					await _dbContext.SaveChangesAsync();

					_cache.Remove("users");
					_cache.Remove("wallets");
					_cache.Remove("userwallets");
					_cache.Remove("transactions");
					_cache.Remove("cryptoitems");

					await transaction.CommitAsync();
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					scope.Dispose();
					throw new Exception($"Error deleting User:{ex}");
				}
				await transaction.DisposeAsync();
				scope.Dispose();
				return $"User with id: {userId} deleted successfully";
			}
			scope.Dispose();
			throw new Exception($"User with id: {userId} not found");
		}

		public async Task<UserViewDTO> GetUserViewDTO(string userId)
		{
			var user = await getUser(userId);
			if (user == null)
			{
				throw new Exception("User not found");
			}
			var userViewDTO = new UserViewDTO
			{
				Id = user.Id,
				UserName = user.UserName,
				Email = user.Email
			};
			return userViewDTO;
		}

		public async Task<string> Register(string username, string email, string password)
		{
			var unique = await isEmailFree(email);
			if (unique)
			{ 
				Guid id = await CreateUser(username, email, password);
				return $"User created successfully with UserId: {id}";
			}
			throw new Exception("Email already in use");
		}

		private async Task<Guid> CreateUser(string username, string email, string password)
		{
			User u = new User
			{
				Id = Guid.NewGuid(),
				UserName = username,
				Email = email,
				Password = password
			};

			Wallet wallet = new Wallet
			{
				Id = Guid.NewGuid(),
				Balance = 10000
			};

			UserWallet userWallet = new UserWallet
			{
				UserId = u.Id,
				WalletId = wallet.Id
			};

			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				await _dbContext.Users.AddAsync(u);
				await _dbContext.SaveChangesAsync();
				await _dbContext.Wallets.AddAsync(wallet);
				await _dbContext.SaveChangesAsync();
				await _dbContext.UserWallets.AddAsync(userWallet);
				await _dbContext.SaveChangesAsync();
				_cache.Remove("users");
				_cache.Remove("wallets");
				await transaction.CommitAsync();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception($"Error creating User and their Wallet: {ex.InnerException.Message}");
			}
			await transaction.DisposeAsync();
			return u.Id;
		}

		private async Task<bool> isEmailFree(string email)
		{
			var users = await ListUsers();
			var user = users.FirstOrDefault(u => u.Email == email);
			if (user == null)
			{
				return true;
			}
			return false;
		}

		public async Task<string> UpdateUser(string userId, string password)
		{
			if (await doesUserExists(userId))
			{
				var transaction = await _dbContext.Database.BeginTransactionAsync();
				try
				{
					User u = await getUser(userId);
					u.Password = password;
					_dbContext.Users.Update(u);
					await _dbContext.SaveChangesAsync();
					_cache.Remove("users");
					await transaction.CommitAsync();
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					throw new Exception("Error updating User:", ex);
				}
				await transaction.DisposeAsync();
				return $"User with id: {userId} updated successfully";
			}
			throw new Exception($"User with id: {userId} not found");
		}

		public async Task<string?> Login(string email, string password)
		{
			var users = await ListUsers();
			var user = users.FirstOrDefault(u => u.Email == email);
			if(user != null)
			{
				if (user.Password == password)
				{
					return $"User successfully logged in with ID: {user.Id.ToString()}, and Username: {user.UserName}";
				}
				throw new Exception("Invalid password");
			}
			throw new Exception("User with email not found");
		}
	}
}
