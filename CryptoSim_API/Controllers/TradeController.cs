using Microsoft.AspNetCore.Mvc;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TradeController : Controller
	{
		[HttpPost("buy")]
		public async Task<IActionResult> BuyCrypto()
		{
			//TODO: Implement buy crypto
			return null;
		}

		[HttpPost("sell")]
		public async Task<IActionResult> SellCrypto() {
			//TODO: Implement sell crypto
			return null;
		}

		[HttpGet("portfolio/{UserId}")]
		public async Task<IActionResult> PrortfolioOfUser() {
			//TODO: Implement get portfolio of user
			return null;
		}
	}
}
