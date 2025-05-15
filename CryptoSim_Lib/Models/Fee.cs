using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSim_Lib.Models
{
    [Table("Fees")]
    public class Fee
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
		public double Percentage { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
	}
}
