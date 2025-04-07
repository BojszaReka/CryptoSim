#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.RepositoryIntefaces
{
	public interface IProfitRepository
	{
		Task<UserProfitDTO> GetUserProfit(string userId);
		Task<DetailedUserProfitDTO> GetDetailedUserProfit(string userId);
	}
}
