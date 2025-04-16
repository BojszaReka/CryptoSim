using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Database
{
	public class CryptoContext : DbContext
	{
		public DbSet<Crypto> Cryptos { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		public DbSet<Wallet> Wallets { get; set; }
		public DbSet<CryptoItem> CryptoItems { get; set; }
		public DbSet<UserWallet> UserWallets { get; set; }

		public CryptoContext(DbContextOptions<CryptoContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Crypto>().HasKey(c => c.Id);
			modelBuilder.Entity<User>().HasKey(u => u.Id);
			modelBuilder.Entity<Transaction>().HasKey(u => u.Id);
			modelBuilder.Entity<Wallet>().HasKey(u => u.Id);
			modelBuilder.Entity<CryptoItem>().HasKey(u => u.Id);
			modelBuilder.Entity<UserWallet>().HasKey(uw => new { uw.UserId, uw.WalletId });

			modelBuilder.Entity<Crypto>().ToTable("Cryptos");
			modelBuilder.Entity<CryptoItem>().ToTable("CryptoItems");
			modelBuilder.Entity<Transaction>().ToTable("Transactions");
			modelBuilder.Entity<User>().ToTable("Users");
			modelBuilder.Entity<Wallet>().ToTable("Wallets");
			modelBuilder.Entity<UserWallet>().ToTable("UserWallets");
		}
	}
}
