using Microsoft.AspNetCore.Mvc;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CryptosController : Controller
	{
		[HttpGet]
		public async Task<IActionResult> GetCryptos()
		{
			//TODO: Implement get cryptos
			return null;
		}

		[HttpGet("{Id}")]
		public async Task<IActionResult> GetCrypto()
		{
			//TOFO: Implement get crypto
			return null;
		}

		[HttpPost]
		public async Task<IActionResult> CreateCrypto() {
			//TODO: Implement create crypto
			return null;
		}

		[HttpDelete("{Id}")]
		public async Task<IActionResult> DeleteCrypto() {
			//TODO: Implement delete crypto
			return null;
		}

	}
}
