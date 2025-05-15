using CryptoSim_API.Lib.UnitOfWork;
using CryptoSim_Lib.Classes;
using CryptoSim_Lib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;


namespace CryptoSim_API.Controllers
{
	/// <summary>
	/// Controller for managing transaction-related operations: retrieve user transaction history and individual transaction details.
	/// </summary>
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class TransactionsController(IUnitOfWork unitOfWork) : Controller
    {
		private IUnitOfWork _unitOfWork = unitOfWork;

		/// <summary>
		/// Retrieves all transactions made by a specific user.
		/// </summary>
		/// <param name="UserId">The ID of the user whose transaction history is requested.</param>
		/// <returns>A response containing a list of the user's transactions.</returns>
		[HttpGet("{UserId}")] 
		public async Task<IActionResult> GetUserTransactions([FromRoute] string UserId) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await _unitOfWork.TransactionRepository.GetUserTransactionsDTO(UserId);
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
		/// Retrieves detailed information about a specific transaction.
		/// </summary>
		/// <param name="TransactionId">The ID of the transaction to retrieve details for.</param>
		/// <returns>A response containing the transaction details.</returns>
		[HttpGet("{TransactionId}")] 
		public async Task<IActionResult> GetTransationDetails([FromRoute] string TransactionId) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await _unitOfWork.TransactionRepository.GetTransactionDetailsDTO(TransactionId);
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
		/// Retrieves a transaction fee report for a specific user.
		/// </summary>
		/// <param name="UserId">The user's unique identifier (GUID).</param>
		/// <returns>
		/// A <see cref="TransactionFeeReportDTO"/> containing the total fees, 
		/// daily breakdown, and individual transaction fee details.
		/// </returns>
		/// <response code="200">Successfully retrieved the transaction fee report.</response>
		/// <response code="400">An error occurred while processing the request.</response>
		[HttpGet("fees/{UserId}")]
		public async Task<IActionResult> GetUserTransactionFees([FromRoute] string UserId)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await _unitOfWork.TransactionRepository.GetUserTransactionReport(UserId);
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
		/// Updates the global transaction fee percentage used for future transactions.
		/// </summary>
		/// <param name="NewFee">
		/// The new fee percentage expressed as a percent value.
		/// For example, pass <c>0.2</c> to set the fee to 0.2%. 
		/// Must be a number between 0 and 50.
		/// </param>
		/// <returns>
		/// A confirmation message indicating that the fee was successfully updated.
		/// </returns>
		/// <response code="200">The transaction fee percentage was successfully updated.</response>
		/// <response code="400">An error occurred while updating the fee percentage.</response>
		[HttpPut("fees")]
		public async Task<IActionResult> ChangeFeeRate([FromBody] double NewFee)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await _unitOfWork.TransactionRepository.ChangeFeeRate(NewFee);
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
