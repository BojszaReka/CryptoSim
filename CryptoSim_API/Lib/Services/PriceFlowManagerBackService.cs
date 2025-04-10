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

		//TODO: Implement price flow manager back service
		protected override async Task ExecuteAsync(CancellationToken stopToken)
		{

			while (!stopToken.IsCancellationRequested)
			{
				try
				{
					using var scope = _scopeFactory.CreateScope();
					var cryptoManagerService = scope.ServiceProvider.GetRequiredService<CryptoManagerService>();

					//decimal percentageChange = (decimal)(_random.NextDouble() * 2 - 1) * 0.05m; // ±5%
					//decimal newPrice = Math.Max(0.01m, crypto.CurrentPrice * (1 + percentageChange));


				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}

				await Task.Delay(TimeSpan.FromSeconds(45), stopToken);
			}
		}
	}
}
