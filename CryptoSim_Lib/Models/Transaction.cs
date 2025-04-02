namespace CryptoSim_Lib.Models
{
	[Table("Transactions")]
	public class Transaction
    {
		[Key, Required]
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid UserId { get; set; }
		public User? User { get; set; }
		public Guid CryptoId { get; set; }
		public Crypto? Crypto { get; set; }
		public double Quantity { get; set; }
		public double Price { get; set; }
		public ETransactionType Type { get; set; }
		public DateTime Date { get; set; } = DateTime.Now;
	}
}
