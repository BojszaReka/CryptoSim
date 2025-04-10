using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSim_Lib.DTO
{
    public class WalletViewDTO
    {
		public string UserName { get; set; }
		public Guid WalletId { get; set; }
		public double Balance { get; set; }
		public List<string> Cryptos { get; set; } = new List<string>();
	}
}
