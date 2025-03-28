using Microsoft.AspNetCore.Mvc;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WalletController : Controller
    {
		[HttpGet("{Id}")] //user id
		public async Task<IActionResult> GetWallet() {
			//TODO: Implement get wallet
			return null;
		}

		[HttpPut("{Id}")] //user id
		public async Task<IActionResult> UpdateWallet() {
			//TODO: Implement update wallet
			return null;
		}

		[HttpDelete("{Id}")] //user id
		public async Task<IActionResult> DeleteWallett() {
			//TODO: Implement delete wallet
			return null;
		}
	}
}
