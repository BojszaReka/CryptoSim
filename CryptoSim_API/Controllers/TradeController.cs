using CryptoSim_API.Lib.UnitOfWork;
using CryptoSim_Lib.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CryptoSim_API.Controllers
{
	/// <summary>
	/// Controller responsible for handling trade operations including buying, selling, and retrieving the user's crypto portfolio.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class TradeController(IUnitOfWork unitOfWork) : Controller
	{
		private IUnitOfWork _unitOfWork = unitOfWork;

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
				response.Message = "Trade create successfully, transaction details:";
				Guid transactiondId = await _unitOfWork.TradeRepository.BuyCrypto(tradeRequest);
				response.Data  = await _unitOfWork.TransactionRepository.GetTransactionDetailsDTO(transactiondId.ToString());
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
				response.Message = "Trade create successfully, transaction details:";
				Guid transactionId = await _unitOfWork.TradeRepository.SellCrypto(tradeRequest);
				response.Data = await _unitOfWork.TransactionRepository.GetTransactionDetailsDTO(transactionId.ToString());
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
		/// <param name="UserId">The ID of the user whose portfolio is being requested.</param>
		/// <returns>A response containing the user's crypto portfolio.</returns>
		[HttpGet("portfolio/{UserId}")]
		public async Task<IActionResult> PrortfolioOfUser([FromRoute] string UserId) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await _unitOfWork.TradeRepository.getUserPortfolio(UserId);
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
		/// Sends cryptocurrency as a gift from one user to another.  
		/// This operation is executed at the current market price and does not incur any transaction fee.  
		/// The gifted amount is deducted from the sender’s wallet and (optionally) added to the receiver’s wallet.  
		/// If the system requires manual acceptance, the gift will initially be pending.
		/// </summary>
		/// <param name="request">A <see cref="GiftRequestDTO"/> containing sender and receiver IDs, crypto ID, and the amount to gift.</param>
		/// <returns>Returns a success message if the gifting is successful or an error message otherwise.</returns>
		/// <remarks>
		/// POST /api/trade/gift  
		/// - The sender must have enough balance to complete the gift.  
		/// - The quantity must be greater than 0.  
		/// - No transaction fee is applied.
		/// </remarks>
		[HttpPost("gift")]
		public async Task<IActionResult> GiftCrypto([FromBody] GiftRequestDTO request)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await _unitOfWork.TradeRepository.giftCrypto(request);
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
		/// Accepts or rejects a pending crypto gift.  
		/// Only the receiver of the gift is authorized to accept or decline it.  
		/// Upon acceptance, the crypto is added to the receiver’s wallet.
		/// </summary>
		/// <param name="giftId">The unique identifier of the gift to be accepted or rejected.</param>
		/// <param name="accepted">Boolean flag indicating whether the gift is accepted (true) or rejected (false).</param>
		/// <returns>Returns a message indicating the result of the acceptance operation.</returns>
		/// <remarks>
		/// PUT /api/trade/accept/{giftId}/{accepted}  
		/// - Only the intended receiver should be allowed to perform this action.  
		/// - If accepted, crypto is transferred to the receiver’s wallet.  
		/// - If rejected, the gift is canceled and the crypto is returned to the sender.
		/// </remarks>
		[HttpPut("accept/{giftId}/{accepted}")]
		public async Task<IActionResult> GiftAcceptance([FromRoute] string giftId, [FromRoute] bool accepted)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await _unitOfWork.TradeRepository.GiftAcceptance(giftId, accepted);
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
