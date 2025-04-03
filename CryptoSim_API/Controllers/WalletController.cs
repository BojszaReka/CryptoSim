using Microsoft.AspNetCore.Mvc;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WalletController : Controller
    {
		[HttpGet("{UserId}")] //user id
		public async Task<IActionResult> GetWallet() {
			//TODO: Implement get wallet
			return null;
		}

		[HttpPut("{UserId}")] //user id
		public async Task<IActionResult> UpdateWallet() {
			//TODO: Implement update wallet
			return null;
		}

		[HttpDelete("{UserId}")] //user id
		public async Task<IActionResult> DeleteWallett() {
			//TODO: Implement delete wallet
			return null;
		}
	}
}
