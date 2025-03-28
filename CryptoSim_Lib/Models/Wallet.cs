namespace CryptoSim_Lib.Models
{
	[Table("Wallets")]
	public class Wallet
    {
		[Required, Key]
		public Guid UserId { get; set; } 
		public decimal Balance { get; set; }
		public List<CryptoItem> Cryptos { get; set; }

		public User? User { get; set; }
	}
}
