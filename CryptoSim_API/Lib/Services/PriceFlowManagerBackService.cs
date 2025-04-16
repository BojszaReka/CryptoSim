#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member


using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CryptoSim_API.Lib.Services
{
	public class PriceFlowManagerBackService : BackgroundService
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private static readonly Random _random = new();

		public PriceFlowManagerBackService(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
		}

		protected override async Task ExecuteAsync(CancellationToken stopToken)
		{
			Console.WriteLine("Background service started...");
			await Task.Delay(TimeSpan.FromSeconds(15), stopToken);
			while (!stopToken.IsCancellationRequested)
			{
				try
				{
					using var scope = _scopeFactory.CreateScope();
					var cryptoManagerService = scope.ServiceProvider.GetRequiredService<ICryptoService>();
					if (cryptoManagerService == null)
					{
						throw new Exception("CryptoManagerService is NOT registered in the DI container.");
					}
					else
					{
						Console.WriteLine("CryptoManagerService was successfully resolved.");
					}

					await cryptoManagerService.RandomBulkUpgrade();
					scope.Dispose();
					
				}
				catch (Exception ex)
				{
					throw new Exception("Error in background service:"+ex.Message);
				}

				await Task.Delay(TimeSpan.FromSeconds(45), stopToken);
			}
		}
	}
}
