using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSim_Lib.DTO
{
    public class TradeRequestDTO
    {
        public Guid UserId { get; set; }
		public Guid CryptoId { get; set; }
        public int Quantity { get; set; }
	}
}
