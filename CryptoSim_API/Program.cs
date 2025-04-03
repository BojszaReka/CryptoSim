namespace CryptoSim_API
{
    public class Program
    {
		public static string ConnectionString { get; private set; }
		public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
			Program.ConnectionString = builder.Configuration.GetConnectionString("SQL");

			// Add services to the container.

			builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<CryptoManagerService>();
            builder.Services.AddScoped<PriceFlowManagerBackService>();
            builder.Services.AddScoped<ProfitManagerService>();
            builder.Services.AddScoped<TradeManagerService>();
            builder.Services.AddScoped<TransationManagerService>();
            builder.Services.AddScoped<UserManagerService>();
            builder.Services.AddScoped<WalletManagerService>();


			var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
