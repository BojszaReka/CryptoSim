using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CryptoSim_Lib.Models
{
    [Table("UserWallets")]
	public class UserWallet
    {
		[Required]
		public Guid UserId { get; set; }
		[Required]
		public Guid WalletId { get; set; }
		[NotMapped, JsonIgnore]
		public User? User { get; set; }
		[NotMapped, JsonIgnore]
		public Wallet? Wallet { get; set; }
	}
}
