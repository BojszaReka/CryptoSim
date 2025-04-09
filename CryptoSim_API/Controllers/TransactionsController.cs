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
	}
}
