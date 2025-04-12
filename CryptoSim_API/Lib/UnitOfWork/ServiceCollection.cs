using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.UnitOfWork
{
	public static class ServiceCollection
	{
		public static void AddLocalServices(this IServiceCollection services)
		{
			services.AddScoped<DataSeederService>();
			services.AddScoped<ICryptoService,CryptoManagerService>();
			services.AddScoped<IUserService,UserManagerService>();
			services.AddScoped<IWalletService, WalletManagerService>();
			services.AddScoped<ITransactionService, TransactionManagerService>();
			services.AddScoped<ITradeService, TradeManagerService>();
			services.AddScoped<IProfitService, ProfitManagerService>();
			services.AddScoped<IUnitOfWork, ProductionUnitOfWork>();
			services.AddHostedService<PriceFlowManagerBackService>();
		}
	}
}
