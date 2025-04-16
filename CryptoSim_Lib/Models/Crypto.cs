using System.Text.Json.Serialization;

namespace CryptoSim_Lib.Models
{
    [Table("Cryptos")]
	public class Crypto
    {
		[Required, Key]
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Name { get; set; }

		[Range(0, double.MaxValue)]
		public double StartingRate { get; set; } //kezdő árfolyam -10eladás +10vétel
		[Range(0, double.MaxValue)]
		public double CurrentPrice { get; set; } //jelenlegi árfolyam -10eladás +10vétel
		[Range(0, double.MaxValue)]
		public double Quantity { get; set; } //mennyiség	
		public bool isDeleted { get; set; } = false; //törölve van-e
		public List<double> PriceHistory { get; set; } = new List<double>(); //árfolyam történet
		[JsonIgnore]
		public List<Transaction>? Transactions { get; set; } = new List<Transaction>(); //tranzakciók
	}
}
