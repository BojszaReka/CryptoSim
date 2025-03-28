namespace CryptoSim_Lib.Models
{
    [Table("Users")]
	public class User
    {
        [Required, Key]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        [EmailAddress]
		public string Email { get; set; }
		public string Password { get; set; }
        public List<Transaction>? Transactions { get; set; }
	}
}
