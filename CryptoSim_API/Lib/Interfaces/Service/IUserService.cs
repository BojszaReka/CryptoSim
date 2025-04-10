
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Interfaces.ServiceInterfaces
{
	public interface IUserService
	{
		Task<bool> doesUserExists(string v);
		Task<string> getUserName(string userId);
	}
}