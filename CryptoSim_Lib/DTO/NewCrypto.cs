using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSim_Lib.DTO
{
    public class NewCrypto
    {
		public string Name { get; set; }
		public double StartingRate { get; set; } 
		public double Quantity { get; set; }
	}
}
