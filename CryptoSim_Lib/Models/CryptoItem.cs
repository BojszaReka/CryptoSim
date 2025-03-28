using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSim_Lib.Models
{
    public class CryptoItem
    {
        public Guid CryptoId { get; set; }
        public Crypto? Crypto { get; set; }
		public double Quantity { get; set; }
	}
}
