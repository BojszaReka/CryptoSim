
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace CryptoSim_API.Lib.Interfaces.ServiceInterfaces
{
	public interface IProfitService
	{
		Task<DetailedUserProfitDTO> GetDetailedUserProfit(string userId);
		Task<UserProfitDTO> GetUserProfit(string userId);
	}
}