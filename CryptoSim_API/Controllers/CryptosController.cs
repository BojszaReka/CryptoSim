using CryptoSim_API.Lib.UnitOfWork;
using CryptoSim_Lib.Classes;
using CryptoSim_Lib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CryptosController : Controller
	{
		private IUnitOfWork _unitOfWork;
		public CryptosController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		/// <summary>
		/// Retrieves a list of all available crypto currencies.
		/// </summary>
		/// <returns>A response containing a list of crypto currency DTOs.</returns>
		[HttpGet]
		public async Task<IActionResult> GetCryptos()
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await _unitOfWork.CryptoRepository.ListCryptosDTO();
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
		/// Retrieves a specific crypto currency by its ID.
		/// </summary>
		/// <param name="CryptoId">The ID of the crypto currency to retrieve.</param>
		/// <returns>A response containing the crypto currency DTO if found.</returns>
		[HttpGet("{CryptoId}")]
		public async Task<IActionResult> GetCrypto([FromRoute] string CryptoId)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await _unitOfWork.CryptoRepository.GetCryptoDTO(CryptoId);
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
		/// Creates a new crypto currency with the provided details.
		/// </summary>
		/// <param name="newCrypto">The data of the new crypto currency to be created.</param>
		/// <returns>A response message indicating the result of the creation process.</returns>
		[HttpPost]
		public async Task<IActionResult> CreateCrypto([FromBody] NewCrypto newCrypto) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await _unitOfWork.CryptoRepository.CreateCrypto(newCrypto);
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
		/// Deletes a crypto currency based on the provided ID.
		/// </summary>
		/// <param name="cryptoId">The ID of the crypto currency to delete.</param>
		/// <returns>A response message indicating the result of the deletion.</returns>
		[HttpDelete("{CryptoId}")]
		public async Task<IActionResult> DeleteCrypto(string cryptoId) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await _unitOfWork.CryptoRepository.DeleteCrypto(cryptoId);
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
