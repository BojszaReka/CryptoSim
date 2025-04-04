using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProfitController : Controller
    {
		ProfitManagerService profitManager;
		public ProfitController(CryptoContext dbContext, IDistributedCache cache)
		{
			profitManager = new ProfitManagerService(dbContext, cache);
		}

		[HttpGet("profit/{UserId}")] 
		public async Task<IActionResult> GetUserProfit(string Id) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await profitManager.GetUserProfit(Id);
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpGet("profit/detail/{UserId}")]
		public async Task<IActionResult> GetDetailedUserProfit(string Id)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await profitManager.GetDetailedUserProfit(Id);
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
