using CryptoSim_API.Lib.UnitOfWork;
using CryptoSim_Lib.Classes;
using CryptoSim_Lib.Models;
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
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await _unitOfWork.UserRepository.Register(username, email, password);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		/// <summary>
		/// Retrieves the details of a user by their user ID.
		/// </summary>
		/// <param name="UserId">The unique identifier of the user to retrieve.</param>
		/// <returns>A response containing the user data if found.</returns>
		[HttpGet("{UserId}")]
		public async Task<IActionResult> GetUser(string UserId)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await _unitOfWork.UserRepository.GetUser(UserId);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		/// <summary>
		/// Updates the password of a specific user by their user ID.
		/// </summary>
		/// <param name="UserId">The unique identifier of the user to update.</param>
		/// <param name="newPassword">The new password of the user</param>
		/// <returns>A response indicating whether the update was successful.</returns>
		[HttpPut("{UserId}")]
		public async Task<IActionResult> UpdateUser([FromRoute] string UserId, string newPassword)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await _unitOfWork.UserRepository.UpdateUser(UserId, newPassword);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		/// <summary>
		/// Deletes a user by their user ID.
		/// </summary>
		/// <param name="UserId">The unique identifier of the user to delete.</param>
		/// <returns>A response indicating whether the deletion was successful.</returns>
		[HttpDelete("{UserId}")]
		public async Task<IActionResult> DeleteUser(string UserId)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await _unitOfWork.UserRepository.DeleteUser(UserId);
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
