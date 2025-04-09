using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSim_Lib.DTO
{
    public class UserViewDTO
    {
		public Guid Id { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; } //TODO: remove password later
	}
}
