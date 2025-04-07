using CryptoSim_API.Lib.UnitOfWork;
using CryptoSim_Lib.Classes;
using CryptoSim_Lib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class TransactionsController : Controller
    {
		private IUnitOfWork _unitOfWork;
		public TransactionsController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		/// <summary>
		/// Retrieves all transactions made by a specific user.
		/// </summary>
		/// <param name="userId">The ID of the user whose transaction history is requested.</param>
		/// <returns>A response containing a list of the user's transactions.</returns>
		[HttpGet("{UserId}")] 
		public async Task<IActionResult> GetUserTransactions(string userId) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await _unitOfWork.TransactionRepository.GetUserTransactionsDTO(userId);
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
		/// <param name="transactionId">The ID of the transaction to retrieve details for.</param>
		/// <returns>A response containing the transaction details.</returns>
		[HttpGet("{TransactionId}")] 
		public async Task<IActionResult> GetTransationDetails(string transactionId) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await _unitOfWork.TransactionRepository.GetTransactionDetailsDTO(transactionId);
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
