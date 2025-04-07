using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSim_Lib.DTO
{
    public class UserTransactionsDTO
    {
        public ETransactionType Type { get; set; }
        public string CryptoName { get; set; }
        public double Quantity { get; set; }
	}
}
