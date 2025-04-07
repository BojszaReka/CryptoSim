using CryptoSim_API.Lib.UnitOfWork;
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
			//TODO: Implement get wallet
			return null;
		}

		/// <summary>
		/// Updates the wallet details for a specific user.
		/// </summary>
		/// <param name="UserId">The unique identifier of the user whose wallet is being updated.</param>
		/// <returns>A response indicating whether the wallet update was successful.</returns>
		[HttpPut("{UserId}")] //user id
		public async Task<IActionResult> UpdateWallet(string UserId) {
			//TODO: Implement update wallet
			return null;
		}

		/// <summary>
		/// Deletes a user's wallet.
		/// </summary>
		/// <param name="UserId">The unique identifier of the user whose wallet is being deleted.</param>
		/// <returns>A response indicating whether the wallet deletion was successful.</returns>
		[HttpDelete("{UserId}")] //user id
		public async Task<IActionResult> DeleteWallett(string UserId) {
			//TODO: Implement delete wallet
			return null;
		}
	}
}
