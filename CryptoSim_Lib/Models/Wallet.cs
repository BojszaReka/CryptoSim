namespace CryptoSim_Lib.Models
{
	[Table("Wallets")]
	public class Wallet
    {
		[Required, Key]
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid UserId { get; set; }
		public decimal Balance { get; set; } = 10000;
		public List<CryptoItem>? Cryptos { get; set; }

		public User? User { get; set; }
	}
}
