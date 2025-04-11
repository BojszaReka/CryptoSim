
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member


namespace CryptoSim_API.Lib.Interfaces.ServiceInterfaces
{
	public interface IUserService
	{
		Task<string> DeleteUser(string userId);
		Task<bool> doesUserExists(string v);
		Task<string> getUserName(string userId);
		Task<UserViewDTO> GetUserViewDTO(string userId);
		Task<string?> Login(string email, string password);
		Task<string> Register(string username, string email, string password);
		Task<string> UpdateUser(string userId, string password);
	}
}