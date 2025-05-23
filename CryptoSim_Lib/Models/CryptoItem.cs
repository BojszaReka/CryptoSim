﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CryptoSim_Lib.Models
{
    [Table("CryptoItems")]
	public class CryptoItem
    {
		[Required, Key]
		public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WalletId { get; set; }
		[JsonIgnore, NotMapped]
		public Wallet? Wallet { get; set; }
		public Guid CryptoId { get; set; }
		[JsonIgnore, NotMapped]
		public Crypto? Crypto { get; set; }
		[Range(0, double.MaxValue)]
		public double Quantity { get; set; }
		[Range(0, double.MaxValue)]
		public double BoughtAtRate { get; set; }
	}
}
