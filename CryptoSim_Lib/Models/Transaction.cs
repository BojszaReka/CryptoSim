using System.Text.Json.Serialization;

namespace CryptoSim_Lib.Models
{
	[Table("Transactions")]
	public class Transaction
    {
		[Key, Required]
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid UserId { get; set; }
		[JsonIgnore]
		public User? User { get; set; }
		public Guid CryptoId { get; set; }
		[JsonIgnore]
		public Crypto? Crypto { get; set; }
		[Range(0, double.MaxValue)]
		public double Quantity { get; set; }
		[Range(0, double.MaxValue)]
		public double Price { get; set; }
		public ETransactionType Type { get; set; }
		public DateTime Date { get; set; } = DateTime.Now;
	}
}
