using CryptoSim_Lib.DTO;
using CryptoSim_Lib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CryptoController : Controller
	{
		CryptoManagerService cryptoManager;
		public CryptoController(CryptoContext dbContext, IDistributedCache cache)
		{
			cryptoManager = new CryptoManagerService(dbContext, cache);
		}

		[HttpPut("price")] 
		public async Task<IActionResult> UpdateCryptoPrice([FromBody] string cryptoId, double price)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await cryptoManager.UpdateCryptoPrice(cryptoId, price);
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpGet("price/history/{CryptoId}")]
		public async Task<IActionResult> GetCryptoPriceHistoy(string cryptoId)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await cryptoManager.GetPriceHistory(cryptoId);
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}
	}
}
