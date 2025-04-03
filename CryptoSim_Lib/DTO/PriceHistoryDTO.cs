using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSim_Lib.DTO
{
    public class PriceHistoryDTO
    {
		public Guid CryptoId { get; set; }
		public string CryptoName { get; set; }	
		public List<double> PriceHistory { get; set; } //árfolyam történet
	}
}
