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
			await seedMockCrypto();
			await seedMockUsers();
			await seedMockWallets();
			await seedMockUserWallets();
			await seedMockCryptoItems();
			await seedMockTransactions();
		}

		public async Task seedMockCrypto()
		{
			if (!_context.Cryptos.Any() || _context.Cryptos.Count() < 10)
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

		public async Task seedMockUsers()
		{
			if (!_context.Users.Any() || _context.Users.Count() < 3)
			{
				if (_context.Users.Count() > 0)
				{
					var users = await _context.Users.ToListAsync();
					_context.Users.RemoveRange(users);
					await _context.SaveChangesAsync();
				}
				var filePath = Path.Combine(_env.ContentRootPath, "UserMockData.json");
				if (File.Exists(filePath))
				{
					var jsonData = await File.ReadAllTextAsync(filePath);
					var users = JsonConvert.DeserializeObject<List<User>>(jsonData);
					var transaction = await _context.Database.BeginTransactionAsync();
					try
					{
						_context.Users.AddRange(users);
						await _context.SaveChangesAsync();
						await transaction.CommitAsync();
					}
					catch (Exception ex)
					{
						await transaction.RollbackAsync();
						throw new Exception("Error seeding User Mock Data", ex);
					}
					await transaction.DisposeAsync();
				}
			}
		}

		public async Task seedMockWallets()
		{
			if (!_context.Wallets.Any() || _context.Wallets.Count() < 3)
			{
				if (_context.Wallets.Count() > 0)
				{
					var wallets = await _context.Wallets.ToListAsync();
					_context.Wallets.RemoveRange(wallets);
					await _context.SaveChangesAsync();
				}
				var filePath = Path.Combine(_env.ContentRootPath, "WalletMockData.json");
				if (File.Exists(filePath))
				{
					var jsonData = await File.ReadAllTextAsync(filePath);
					var wallets = JsonConvert.DeserializeObject<List<Wallet>>(jsonData);
					var transaction = await _context.Database.BeginTransactionAsync();
					try
					{
						_context.Wallets.AddRange(wallets);
						await _context.SaveChangesAsync();
						await transaction.CommitAsync();
					}
					catch (Exception ex)
					{
						await transaction.RollbackAsync();
						throw new Exception("Error seeding Wallet Mock Data", ex);
					}
					await transaction.DisposeAsync();
				}
			}
		}

		public async Task seedMockUserWallets()
		{
			if (!_context.UserWallets.Any() || _context.UserWallets.Count() < 3)
			{
				if (_context.UserWallets.Count() > 0)
				{
					var userWallets = await _context.UserWallets.ToListAsync();
					_context.UserWallets.RemoveRange(userWallets);
					await _context.SaveChangesAsync();
				}
				var filePath = Path.Combine(_env.ContentRootPath, "UserWalletMockData.json");
				if (File.Exists(filePath))
				{
					var jsonData = await File.ReadAllTextAsync(filePath);
					var userWallets = JsonConvert.DeserializeObject<List<UserWallet>>(jsonData);
					var transaction = await _context.Database.BeginTransactionAsync();
					try
					{
						_context.UserWallets.AddRange(userWallets);
						await _context.SaveChangesAsync();
						await transaction.CommitAsync();
					}
					catch (Exception ex)
					{
						await transaction.RollbackAsync();
						throw new Exception("Error seeding UserWallet Mock Data", ex);
					}
					await transaction.DisposeAsync();
				}
			}
		}

		public async Task seedMockCryptoItems()
		{
			if (!_context.CryptoItems.Any() || _context.CryptoItems.Count() < 9)
			{
				if (_context.CryptoItems.Count() > 0)
				{
					var cryptoItems = await _context.CryptoItems.ToListAsync();
					_context.CryptoItems.RemoveRange(cryptoItems);
					await _context.SaveChangesAsync();
				}
				var filePath = Path.Combine(_env.ContentRootPath, "CryptoItemMockData.json");
				if (File.Exists(filePath))
				{
					var jsonData = await File.ReadAllTextAsync(filePath);
					var cryptoItems = JsonConvert.DeserializeObject<List<CryptoItem>>(jsonData);
					var transaction = await _context.Database.BeginTransactionAsync();
					try
					{
						_context.CryptoItems.AddRange(cryptoItems);
						await _context.SaveChangesAsync();
						await transaction.CommitAsync();
					}
					catch (Exception ex)
					{
						await transaction.RollbackAsync();
						throw new Exception("Error seeding CryptoItem Mock Data", ex);
					}
					await transaction.DisposeAsync();
				}
			}
		}

		public async Task seedMockTransactions()
		{
			if (!_context.Transactions.Any() || _context.Transactions.Count() < 8)
			{
				if (_context.Transactions.Count() > 0)
				{
					var transactions = await _context.Transactions.ToListAsync();
					_context.Transactions.RemoveRange(transactions);
					await _context.SaveChangesAsync();
				}
				var filePath = Path.Combine(_env.ContentRootPath, "TransactionMockData.json");
				if (File.Exists(filePath))
				{
					var jsonData = await File.ReadAllTextAsync(filePath);
					var transactions = JsonConvert.DeserializeObject<List<Transaction>>(jsonData);
					var transaction = await _context.Database.BeginTransactionAsync();
					try
					{
						_context.Transactions.AddRange(transactions);
						await _context.SaveChangesAsync();
						await transaction.CommitAsync();
					}
					catch (Exception ex)
					{
						await transaction.RollbackAsync();
						throw new Exception("Error seeding Transaction Mock Data", ex);
					}
					await transaction.DisposeAsync();
				}
			}
		}

	}
}
