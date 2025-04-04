using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSim_Lib.DTO
{
    public class UserPortfolioDTO
    {
        public string UserName { get; set; }
        public List<PortfolioItem> Cryptos { get; set; } = new List<PortfolioItem>();
	}

    public class PortfolioItem
    {
        public string CryptoName { get; set; }
        public double Quantity { get; set; }
        public double CurrentValue { get; set; }
	}
}
