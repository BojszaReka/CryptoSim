using CryptoSim_API.Lib.RepositoryIntefaces;

namespace CryptoSim_API.Lib.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly CryptoContext _dbContext;
		private readonly IDistributedCache _cache;
		UserManagerService _userManager;
		public UserRepository(CryptoContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
			_userManager = new UserManagerService(_dbContext, _cache);
		}

		public async Task<string> DeleteUser(string UserId)
		{
			return await _userManager.DeleteUser(UserId);
		}

		public async Task<UserViewDTO> GetUser(string UserId)
		{
			return await _userManager.GetUserViewDTO(UserId);
		}

		public async Task<string> Register(string username, string email, string password)
		{
			return await _userManager.Register(username, email, password);
		}

		public async Task<string> UpdateUser(string UserId, string password)
		{
			return await _userManager.UpdateUser(UserId, password);
		}
	}
}
