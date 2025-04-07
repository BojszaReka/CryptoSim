namespace CryptoSim_API.Lib.RepositoryIntefaces
{
	public interface IProfitRepository
	{
		Task<UserProfitDTO> GetUserProfit(string userId);
		Task<DetailedUserProfitDTO> GetDetailedUserProfit(string userId);
	}
}
