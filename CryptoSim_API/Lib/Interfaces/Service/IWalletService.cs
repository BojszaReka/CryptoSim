
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member


namespace CryptoSim_API.Lib.Interfaces.ServiceInterfaces
{
	public interface IWalletService
	{
		Task AddCryptoToUserWallet(string v1, string v2, int quantity);
		Task DecreaseUserBalance(string v, double cost);
		Task<string> DeleteWallet(string userId);
		Task<string> DeleteWalletData(string userId);
		Task<bool> doesUserHasBalance(string v, double cost);
		Task<bool> doesUserHasCryptoBalance(string v1, string v2, int quantity);
		Task<bool> doesWalletExists(string walletId);
		Task<List<PortfolioItem>> getUserWalletAsPortfolioList(string userId);
		Task<UserWallet> GetUserWalletByUserId(string userId);
		Task<Wallet> GetWalletByUserId(string userId);
		Task<WalletViewDTO> GetWalletViewDTO(string userId);
		Task IncreaseUserBalance(string v, double cost);
		Task RemoveCryptoFromUserWallet(string v1, string v2, int quantity);
		Task<string> UpdateWallet(WalletUpdateDTO wallet);
	}
}