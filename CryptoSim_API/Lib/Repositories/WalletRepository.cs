using CryptoSim_API.Lib.Interfaces.RepositoryIntefaces;
using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Caching.Memory;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Repositories
{
	public class WalletRepository : IWalletRepository
	{
		private readonly IServiceScopeFactory _scopeFactory;
		public WalletRepository(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
		}

		private IWalletService GetService()
		{
			var scope = _scopeFactory.CreateScope();
			var _manager = scope.ServiceProvider.GetRequiredService<IWalletService>();
			return _manager;
		}
		public async Task<string> DeleteWallet(string userId)
		{
			var _walletManager = GetService();
			return await _walletManager.DeleteWalletData(userId);
		}

		public async Task<WalletViewDTO> GetWallet(string userId)
		{
			var _walletManager = GetService();
			return await _walletManager.GetWalletViewDTO(userId);
		}

		public async Task<string> UpdateWallet(WalletUpdateDTO wallet)
		{
			var _walletManager = GetService();
			return await _walletManager.UpdateWallet(wallet);
		}
	}
}
