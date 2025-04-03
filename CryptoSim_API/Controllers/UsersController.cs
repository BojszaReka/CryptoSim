using Microsoft.AspNetCore.Mvc;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : Controller
    {

        [HttpPost("register")]
        public async Task<IActionResult> Register(String username, String email, String password)
		{
			//TODO: Implement register
			return null;
		}

		[HttpGet("{UserId}")]
		public async Task<IActionResult> GetUser(String Id)
		{
			//TODO: Implement get user
			return null;
		}

		[HttpPut("{UserId}")]
		public async Task<IActionResult> UpdateUser()
		{
			//TODO: Implement update user
			return null;
		}

		[HttpDelete("{UserId}")]
		public async Task<IActionResult> DeleteUser()
		{
			//TODO: Implement delete user
			return null;
		}

	}
}
