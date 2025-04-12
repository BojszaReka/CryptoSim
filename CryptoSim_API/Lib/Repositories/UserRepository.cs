using CryptoSim_API.Lib.Interfaces.RepositoryIntefaces;
using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Caching.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly IServiceScopeFactory _scopeFactory;
		public UserRepository(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
		}

		private IUserService GetService()
		{
			var scope = _scopeFactory.CreateScope();
			var _manager = scope.ServiceProvider.GetRequiredService<IUserService>();
			return _manager;
		}

		public async Task<string> DeleteUser(string UserId)
		{
			var _userManager = GetService();
			return await _userManager.DeleteUser(UserId);
		}

		public async Task<UserViewDTO> GetUser(string UserId)
		{
			var _userManager = GetService();
			return await _userManager.GetUserViewDTO(UserId);
		}

		public async Task<string?> Login(string email, string password)
		{
			var _userManager = GetService();
			return await _userManager.Login(email, password);
		}

		public async Task<string> Register(string username, string email, string password)
		{
			var _userManager = GetService();
			return await _userManager.Register(username, email, password);
		}

		public async Task<string> UpdateUser(string UserId, string password)
		{
			var _userManager = GetService();
			return await _userManager.UpdateUser(UserId, password);
		}
	}
}
