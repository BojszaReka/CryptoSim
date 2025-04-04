using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSim_Lib.DTO
{
    public class NewTransactionDTO
    {
		public Guid UserId { get; set; }
		public Guid CryptoId { get; set; }
		public double Quantity { get; set; }
		public double Price { get; set; }
		public ETransactionType Type { get; set; }
		public DateTime Date { get; set; } = DateTime.Now;
	}
}
