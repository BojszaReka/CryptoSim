using CryptoSim_Lib.DTO;
using CryptoSim_Lib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CryptosController : Controller
	{
		CryptoManagerService cryptoManager;
		public CryptosController(CryptoContext dbContext, IDistributedCache cache)
		{
			cryptoManager = new CryptoManagerService(dbContext, cache);
		}

		[HttpGet]
		public async Task<IActionResult> GetCryptos()
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await cryptoManager.ListCryptosDTO();
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpGet("{CryptoId}")]
		public async Task<IActionResult> GetCrypto(string Id)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await cryptoManager.GetCryptoDTO(Id);
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpPost]
		public async Task<IActionResult> CreateCrypto([FromBody] NewCrypto newCrypto) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await cryptoManager.CreateCrypto(newCrypto);
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpDelete("{CryptoId}")]
		public async Task<IActionResult> DeleteCrypto(string cryptoId) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await cryptoManager.DeleteCrypto(cryptoId);
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
