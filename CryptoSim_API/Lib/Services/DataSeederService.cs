using CryptoSim_Lib.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CryptoSim_API.Lib.Services
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public class DataSeederService
	{
		private readonly CryptoContext _context;
		private readonly IWebHostEnvironment _env;

		public DataSeederService(CryptoContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}
		public async Task SeedAsync()
		{
			if (!_context.Cryptos.Any() || _context.Cryptos.Count()<10)
			{
				if (_context.Cryptos.Count() > 0)
				{
					var cryptos = await _context.Cryptos.ToListAsync();
					_context.Cryptos.RemoveRange(cryptos);
					await _context.SaveChangesAsync();
				}
				var filePath = Path.Combine(_env.ContentRootPath, "CryptoMockData.json");
				if (File.Exists(filePath))
				{
					var jsonData = await File.ReadAllTextAsync(filePath);
					var cryptos = JsonConvert.DeserializeObject<List<Crypto>>(jsonData);

					var transaction = await _context.Database.BeginTransactionAsync();
					try
					{
						_context.Cryptos.AddRange(cryptos);
						await _context.SaveChangesAsync();
						await transaction.CommitAsync();
					}
					catch (Exception ex)
					{
						await transaction.RollbackAsync();
						throw new Exception("Error seeding Crypto Mock Data", ex);
					}			
					await transaction.DisposeAsync();
				}
			}
		}

	}
}
