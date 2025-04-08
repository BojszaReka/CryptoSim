namespace CryptoSim_API.Lib.RepositoryIntefaces
{
	public interface IUserRepository
	{
		Task<string> Register(string username, string email, string password);
		Task<UserViewDTO> GetUser(string UserId);
		Task<string> UpdateUser(string UserId, string password);
		Task<string> DeleteUser(string UserId);
	}
}
