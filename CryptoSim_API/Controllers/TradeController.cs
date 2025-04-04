using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TradeController : Controller
	{
		TradeManagerService tradeManager;
		public TradeController(CryptoContext dbContext, IDistributedCache cache)
		{
			tradeManager = new TradeManagerService(dbContext, cache);
		}

		[HttpPost("buy")]
		public async Task<IActionResult> BuyCrypto(TradeRequestDTO tradeRequest)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await tradeManager.BuyCrypto(tradeRequest);
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpPost("sell")]
		public async Task<IActionResult> SellCrypto(TradeRequestDTO tradeRequest) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await tradeManager.SellCrypto(tradeRequest);
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpGet("portfolio/{UserId}")]
		public async Task<IActionResult> PrortfolioOfUser(String userId) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await tradeManager.getUserPortfolio(userId);
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
