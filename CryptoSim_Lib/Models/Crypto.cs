﻿namespace CryptoSim_Lib.Models
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
		public List<double> PriceHistory { get; set; } = new List<double>(); //árfolyam történet
		public List<Transaction>? Transactions { get; set; } = new List<Transaction>(); //tranzakciók
	}
}
