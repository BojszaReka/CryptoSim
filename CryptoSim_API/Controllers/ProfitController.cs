using Microsoft.AspNetCore.Mvc;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProfitController : Controller
    {
		[HttpGet("profit/{Id}")] //user id
		public async Task<IActionResult> GetUserProfit() {
			//TODO: Implement get user profit
			return null;
		}

		[HttpGet("profit/detail/{Id}")] //user id
		public async Task<IActionResult> GetDetailedUserProfit()
		{
			//TODO: Implement get user profit
			return null;
		}
	}
}
