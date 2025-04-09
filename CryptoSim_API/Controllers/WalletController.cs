using CryptoSim_API.Lib.UnitOfWork;
using CryptoSim_Lib.Classes;
using CryptoSim_Lib.Models;
using Microsoft.AspNetCore.Mvc;

namespace CryptoSim_API.Controllers
{
	/// <summary>
	/// Controller for managing wallet-related operations: retrieving, updating, and deleting a user's wallet.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class WalletController(IUnitOfWork unitOfWork) : Controller
    {
		private IUnitOfWork _unitOfWork = unitOfWork;

		/// <summary>
		/// Retrieves the wallet details of a specific user.
		/// </summary>
		/// <param name="UserId">The unique identifier of the user whose wallet is being retrieved.</param>
		/// <returns>A response containing the user's wallet information.</returns>
		[HttpGet("{UserId}")] //user id
		public async Task<IActionResult> GetWallet(string UserId) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await _unitOfWork.WalletRepository.GetWallet(UserId);
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
		/// Updates the wallet details for a specific user.
		/// </summary>
		/// <param name="updateDTO">The ID of the user whose wallet is being updated and the new balance.</param>
		/// <returns>A response indicating whether the wallet update was successful.</returns>
		[HttpPut]
		public async Task<IActionResult> UpdateWallet(WalletUpdateDTO updateDTO) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await _unitOfWork.WalletRepository.UpdateWallet(updateDTO);
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
		/// Deletes a user's wallet.
		/// </summary>
		/// <param name="UserId">The unique identifier of the user whose wallet is being deleted.</param>
		/// <returns>A response indicating whether the wallet deletion was successful.</returns>
		[HttpDelete("{UserId}")] //user id
		public async Task<IActionResult> DeleteWallett(string UserId) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await _unitOfWork.WalletRepository.DeleteWallet(UserId);
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
