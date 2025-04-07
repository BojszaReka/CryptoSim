using CryptoSim_API.Lib.UnitOfWork;
using CryptoSim_Lib.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TradeController : Controller
	{
		private IUnitOfWork _unitOfWork;
		public TradeController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		/// <summary>
		/// Executes a crypto buy transaction for a user.
		/// </summary>
		/// <param name="tradeRequest">The details of the trade including user ID, crypto ID, and quantity.</param>
		/// <returns>A response message indicating the result of the buy operation.</returns>
		[HttpPost("buy")]
		public async Task<IActionResult> BuyCrypto(TradeRequestDTO tradeRequest)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await _unitOfWork.TradeRepository.BuyCrypto(tradeRequest);
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
		/// Executes a crypto sell transaction for a user.
		/// </summary>
		/// <param name="tradeRequest">The details of the trade including user ID, crypto ID, and quantity.</param>
		/// <returns>A response message indicating the result of the sell operation.</returns>
		[HttpPost("sell")]
		public async Task<IActionResult> SellCrypto(TradeRequestDTO tradeRequest) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await _unitOfWork.TradeRepository.SellCrypto(tradeRequest);
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
		/// Retrieves the crypto portfolio for a specific user, including their owned assets and quantities.
		/// </summary>
		/// <param name="userId">The ID of the user whose portfolio is being requested.</param>
		/// <returns>A response containing the user's crypto portfolio.</returns>
		[HttpGet("portfolio/{UserId}")]
		public async Task<IActionResult> PrortfolioOfUser(String userId) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await _unitOfWork.TradeRepository.getUserPortfolio(userId);
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
