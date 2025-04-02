namespace CryptoSim_Lib.Models
{
    [Table("Cryptos")]
	public class Crypto
    {
		[Required, Key]
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Name { get; set; }
        public double StartingRate { get; set; } //kezdő árfolyam -10eladás +10vétel
		public double CurrentPrice { get; set; } //jelenlegi árfolyam -10eladás +10vétel
		public double Quantity { get; set; } //mennyiség	
		public List<double> PriceHistory { get; set; } //árfolyam történet
		public List<Transaction>? Transactions { get; set; } //tranzakciók
	}
}
