using Microsoft.AspNetCore.Mvc;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CryptoController : Controller
	{
		//cryptosmanager-ben kell implementálni

		[HttpPut("price")] 
		public async Task<IActionResult> UpdateCryptoPrice()
		{
			//TODO: Implement update crypto price	
			return null;
		}

		[HttpGet("price/history/{CryptoId}")] //crypto id
		public async Task<IActionResult> GetCryptoPriceHistoy()
		{
			//TODO: Implement get crypto price history
			return null;
		}
	}
}
