using Microsoft.Extensions.Caching.Distributed;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Services
{
	public class UserManagerService
	{
		//TODO: Implement user manager service
		private readonly CryptoContext _dbContext;
		private readonly IDistributedCache _cache;
		public UserManagerService(CryptoContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
		}

		public async Task<User> getUser(string Id)
		{
			//TODO: Implement getUser
			throw new NotImplementedException("UserManagerService.getUser not implemented");
		}

		public async Task<string> getUserName(string Id)
		{
			//TODO: Implement getUserName
			throw new NotImplementedException("UserManagerService.getUserName not implemented");
		}

		public async Task<bool> doesUserExists(string Id)
		{
			//TODO: Implement doesUserExists
			/*
			var crypto = await GetCrypto(Id);
			return crypto != null;
			*/
			throw new NotImplementedException("UserManagerService.doesUserExists not implemented");
		}

		internal async Task<string> DeleteUser(string userId)
		{
			//TODO: Implement DeleteUser
			throw new NotImplementedException();
		}

		internal async Task<UserViewDTO> GetUserViewDTO(string userId)
		{
			//TODO: Implement GetUserViewDTO
			throw new NotImplementedException();
		}

		internal async Task<string> Register(string username, string email, string password)
		{
			//TODO: Implement Register
			throw new NotImplementedException();
		}

		internal async Task<string> UpdateUser(string userId, string password)
		{
			//TODO: Implement UpdateUser
			throw new NotImplementedException();
		}
	}
}
