﻿using CryptoSim_API.Lib.Interfaces.RepositoryIntefaces;
using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Caching.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;
		IUserService _userManager;
		public UserRepository(CryptoContext dbContext, IMemoryCache cache, IUserService userservice)
		{
			_dbContext = dbContext;
			_cache = cache;
			_userManager = userservice;
		}

		public async Task<string> DeleteUser(string UserId)
		{
			return await _userManager.DeleteUser(UserId);
		}

		public async Task<UserViewDTO> GetUser(string UserId)
		{
			return await _userManager.GetUserViewDTO(UserId);
		}

		public async Task<string?> Login(string email, string password)
		{
			return await _userManager.Login(email, password);
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
