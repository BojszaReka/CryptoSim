using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSim_Lib.Classes
{
    public class CacheTimeoutOptions
    {
		public const string SectionName = "CacheTimeoutOptions";
		public int ShortLivedTimeInSeconds { get; set; } = 1;
		public int MediumLivedTimeInSeconds { get; set; } = 1;
		public int LongLivedTimeInSeconds { get; set; } = 1;
	}
}
