using System.Text.Json.Serialization;

namespace CryptoSim_Lib.Models
{
	[Table("Wallets")]
	public class Wallet
    {
		[Required, Key]
		public Guid Id { get; set; } = Guid.NewGuid();
		public double Balance { get; set; } = 10000;
		[NotMapped, JsonIgnore]
		public List<CryptoItem>? Cryptos { get; set; } = new List<CryptoItem>();
		[NotMapped, JsonIgnore]
		public List<UserWallet>? UserWallets { get; set; } 
	}
}
