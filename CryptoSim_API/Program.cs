using CryptoSim_API.Lib.UnitOfWork;
using CryptoSim_Lib.Classes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API
{
    public class Program
    {
		internal static string? ConnectionString { get; private set; }
		public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
			Program.ConnectionString = builder.Configuration.GetConnectionString("SQL");

			builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(options =>
			{
				var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

				options.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "Crypto Simulation API",
					Description = "An ASP.NET Core Web API for the simulation of buying and selling of crypto currency"
				});
			});

			builder.Services.AddDbContext<CryptoContext>(options =>options.UseSqlServer(Program.ConnectionString));
			builder.Services.AddMemoryCache();

			var timeoutOptions = new CacheTimeoutOptions();
			builder.Configuration.GetSection(CacheTimeoutOptions.SectionName).Bind(timeoutOptions);
			var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(timeoutOptions.MediumLivedTimeInSeconds));

			
			builder.Services.AddScoped<DataSeederService>();
			builder.Services.AddScoped<CryptoManagerService>();
            builder.Services.AddScoped<PriceFlowManagerBackService>();
            builder.Services.AddScoped<ProfitManagerService>();
            builder.Services.AddScoped<TradeManagerService>();
            builder.Services.AddScoped<TransactionManagerService>();
            builder.Services.AddScoped<UserManagerService>();
            builder.Services.AddScoped<WalletManagerService>();
			builder.Services.AddScoped<IUnitOfWork, ProductionUnitOfWork>();

			// Update the following code block to ensure the Newtonsoft.Json package is used correctly
			builder.Services.AddControllers().AddNewtonsoftJson(options =>{	options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;});

			var app = builder.Build();

			using (var scope = app.Services.CreateScope())
			{
				var seeder = scope.ServiceProvider.GetRequiredService<DataSeederService>();
				await seeder.SeedAsync();
			}


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
