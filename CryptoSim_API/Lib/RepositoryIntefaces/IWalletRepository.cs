#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.RepositoryIntefaces
{
	public interface IWalletRepository
	{
		Task<WalletViewDTO> GetWallet(string userId);
		Task<string> UpdateWallet(WalletUpdateDTO wallet);
		Task<string> DeleteWallet(string userId);
	}
}
