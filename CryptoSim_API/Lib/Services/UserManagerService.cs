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
			throw new NotImplementedException("UserManagerService.getUser not implemented");
		}

		public async Task<string> getUserName(string Id)
		{
			throw new NotImplementedException("UserManagerService.getUserName not implemented");
		}

		public async Task<bool> doesUserExists(string Id)
		{
			/*
			var crypto = await GetCrypto(Id);
			return crypto != null;
			*/
			throw new NotImplementedException("UserManagerService.doesUserExists not implemented");
		}
	}
}
