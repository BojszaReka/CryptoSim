using CryptoSim_API.Lib.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace CryptoSim_API.Controllers
{
	/// <summary>
	/// Controller for managing user-related operations: user registration, retrieval, updates, and deletion.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController(IUnitOfWork unitOfWork) : Controller
    {
		private IUnitOfWork _unitOfWork = unitOfWork;

		/// <summary>
		/// Registers a new user with the specified username, email, and password.
		/// </summary>
		/// <param name="username">The username for the new user.</param>
		/// <param name="email">The email address of the new user.</param>
		/// <param name="password">The password for the new user.</param>
		/// <returns>A result indicating whether the registration was successful.</returns>
		[HttpPost("register")]
        public async Task<IActionResult> Register(string username, string email, string password)
		{
			//TODO: Implement register
			return null;
		}

		/// <summary>
		/// Retrieves the details of a user by their user ID.
		/// </summary>
		/// <param name="UserId">The unique identifier of the user to retrieve.</param>
		/// <returns>A response containing the user data if found.</returns>
		[HttpGet("{UserId}")]
		public async Task<IActionResult> GetUser(string UserId)
		{
			//TODO: Implement get user
			return null;
		}

		/// <summary>
		/// Updates the information of a specific user by their user ID.
		/// </summary>
		/// <param name="UserId">The unique identifier of the user to update.</param>
		/// <returns>A response indicating whether the update was successful.</returns>
		[HttpPut("{UserId}")]
		public async Task<IActionResult> UpdateUser(string UserId)
		{
			//TODO: Implement update user
			return null;
		}

		/// <summary>
		/// Deletes a user by their user ID.
		/// </summary>
		/// <param name="UserId">The unique identifier of the user to delete.</param>
		/// <returns>A response indicating whether the deletion was successful.</returns>
		[HttpDelete("{UserId}")]
		public async Task<IActionResult> DeleteUser(string UserId)
		{
			//TODO: Implement delete user
			return null;
		}

	}
}
