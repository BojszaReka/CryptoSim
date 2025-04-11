#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member


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
			while (!stopToken.IsCancellationRequested)
			{
				try
				{
					using var scope = _scopeFactory.CreateScope();
					var cryptoManagerService = scope.ServiceProvider.GetRequiredService<CryptoManagerService>();

					var cryptos = await cryptoManagerService.ListCryptos();
					foreach (var crypto in cryptos)
					{
						Console.WriteLine($"Updating price for {crypto.Name} ({crypto.Id})...");
						double percentageChange = (double)(_random.NextDouble() * 2 - 1) * 0.05d; // ±5%
						double newPrice = Math.Max(0.01d, crypto.CurrentPrice * (1 + percentageChange));
						await cryptoManagerService.UpdateCryptoPrice(crypto.Id.ToString(), newPrice);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error in background service:"+ex.Message);
				}

				await Task.Delay(TimeSpan.FromSeconds(45), stopToken);
			}
		}
	}
}
