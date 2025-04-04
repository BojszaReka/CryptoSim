using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSim_Lib.Models
{
    [Table("CryptoItems")]
	public class CryptoItem
    {
		[Required, Key]
		public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WalletId { get; set; }
		public Wallet? Wallet { get; set; }
		public Guid CryptoId { get; set; }
        public Crypto? Crypto { get; set; }
		public double Quantity { get; set; }
		public double BoughtAtRate { get; set; }
	}
}
