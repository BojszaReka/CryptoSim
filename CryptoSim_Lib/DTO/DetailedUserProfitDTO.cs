using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSim_Lib.DTO
{
    public class DetailedUserProfitDTO
    {
        public string UserName { get; set; }
        public List<ProfitItem> Profits { get; set; }
    }

    public class ProfitItem {
        public string CryptoName { get; set; }
        public double Profit { get; set; }
	}
}
