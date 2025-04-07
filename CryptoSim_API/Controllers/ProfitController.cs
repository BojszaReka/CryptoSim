using CryptoSim_API.Lib.UnitOfWork;
using CryptoSim_Lib.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CryptoSim_API.Controllers
{
	/// <summary>
	/// Controller for managing user profit-related operations: retrieving total and detailed profit information for users.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class ProfitController(IUnitOfWork unitOfWork) : Controller
    {
		private IUnitOfWork _unitOfWork = unitOfWork;

		/// <summary>
		/// Retrieves the total profit for a specific user.
		/// </summary>
		/// <param name="UserId">The ID of the user whose profit is being requested.</param>
		/// <returns>A response containing the user's total profit.</returns>
		[HttpGet("profit/{UserId}")] 
		public async Task<IActionResult> GetUserProfit(string UserId) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await _unitOfWork.ProfitRepository.GetUserProfit(UserId);
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		/// <summary>
		/// Retrieves a detailed breakdown of the user's profit, including transaction history or specific gains.
		/// </summary>
		/// <param name="UserId">The ID of the user for whom detailed profit information is requested.</param>
		/// <returns>A response containing detailed profit data for the user.</returns>
		[HttpGet("profit/detail/{UserId}")]
		public async Task<IActionResult> GetDetailedUserProfit(string UserId)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await _unitOfWork.ProfitRepository.GetDetailedUserProfit(UserId);
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
