using System.Text.Json.Serialization;

namespace CryptoSim_Lib.Models
{
    [Table("Users")]
	public class User
    {
		[Required, Key]
        public Guid Id { get; set; } = Guid.NewGuid();
		[StringLength(70, MinimumLength = 5)]
		public string UserName { get; set; }
        [EmailAddress(ErrorMessage = "Email address format is incorrect")]
		public string Email { get; set; }
		[StringLength(70, MinimumLength = 10)]
		public string Password { get; set; }
		[NotMapped, JsonIgnore]
		public List<Transaction>? Transactions { get; set; } = new List<Transaction>();
		[NotMapped, JsonIgnore]
		public List<UserWallet>? UserWallets { get; set; } = new List<UserWallet>();
	}
}
