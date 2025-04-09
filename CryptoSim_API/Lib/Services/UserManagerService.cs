using CryptoSim_Lib.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Transactions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Services
{
	public class UserManagerService
	{
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;
		public UserManagerService(CryptoContext dbContext, IMemoryCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
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

		internal async Task<string> DeleteUser(string userId)
		{
			WalletManagerService _walletManager = new WalletManagerService(_dbContext, _cache);
			TransactionManagerService _transactionManager = new TransactionManagerService(_dbContext, _cache);

			if (await doesUserExists(userId))
			{
				var transaction = await _dbContext.Database.BeginTransactionAsync();
				try
				{
					User u = await getUser(userId);
					_dbContext.Users.Remove(u);
					await _dbContext.SaveChangesAsync();
					_cache.Remove("users");

					await _walletManager.DeleteWallet(userId);
					await _transactionManager.DeleteUserTransactions(userId);

					await transaction.CommitAsync();
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					throw new Exception("Error deleting User:", ex);
				}
				await transaction.DisposeAsync();

				return $"User with id: {userId} deleted successfully";
			}
			throw new Exception($"User with id: {userId} not found");
		}

		internal async Task<UserViewDTO> GetUserViewDTO(string userId)
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
				Email = user.Email,
				Password = user.Password //TODO: remove password later
			};
			return userViewDTO;
		}

		internal async Task<string> Register(string username, string email, string password)
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
			WalletManagerService walletManager = new WalletManagerService(_dbContext, _cache);

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
				UserId = u.Id,
				Balance = 10000
			};

			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				await _dbContext.Users.AddAsync(u);
				await _dbContext.SaveChangesAsync();
				await _dbContext.Wallets.AddAsync(wallet);
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

		internal async Task<string> UpdateUser(string userId, string password)
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

		internal async Task<string?> Login(string email, string password)
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
