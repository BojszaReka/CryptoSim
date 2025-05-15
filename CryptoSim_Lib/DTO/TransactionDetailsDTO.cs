using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSim_Lib.DTO
{
    public class TransactionDetailsDTO
    {
		public string UserName { get; set; }
		public string CryptoName { get; set; }
		public double Quantity { get; set; }
		public double Price { get; set; }
		public double Fee { get; set; }
		public string Type { get; set; }
		public DateTime Date { get; set; }
	}
}
