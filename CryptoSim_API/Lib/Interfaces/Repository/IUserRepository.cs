#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Interfaces.RepositoryIntefaces
{
	public interface IUserRepository
	{
		Task<string> Register(string username, string email, string password);
		Task<UserViewDTO> GetUser(string UserId);
		Task<string> UpdateUser(string UserId, string password);
		Task<string> DeleteUser(string UserId);
		Task<string?> Login(string email, string password);
	}
}
