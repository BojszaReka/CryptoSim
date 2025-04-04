using Microsoft.Extensions.Caching.Distributed;

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

		public async Task<string> getUserName(string Id)
		{
			throw new NotImplementedException("UserManagerService.getUserName not implemented");
		}
	}
}
