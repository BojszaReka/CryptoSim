using Microsoft.EntityFrameworkCore;

namespace CryptoSim_API.Lib.Database
{
	public class CryptoContext : DbContext
	{
		//public DbSet<Crypto> Cryptos { get; set; }
		//public DbSet<User> Users { get; set; }

		public CryptoContext() : base(new DbContextOptionsBuilder().UseSqlServer(Program.ConnectionString).Options)
		{

		}
	}
}
