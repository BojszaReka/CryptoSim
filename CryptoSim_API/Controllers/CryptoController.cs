﻿using CryptoSim_API.Lib.UnitOfWork;
using CryptoSim_Lib.Classes;
using CryptoSim_Lib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CryptoController : Controller
	{
		private IUnitOfWork _unitOfWork;
		public CryptoController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}


		/// <summary>
		/// Updates the price of a crypto currency
		/// </summary>
		/// <param name="cryptoId">Id of the crypto currency to modify</param>
		/// <param name="price">The new price of the crpyto currency</param>
		/// <returns>A status code and a message about the result</returns>
		[HttpPut("price")] 
		public async Task<IActionResult> UpdateCryptoPrice(string cryptoId, double price)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Message = await _unitOfWork.CryptoRepository.UpdateCryptoPrice(cryptoId, price);
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
		/// Returns the price history of the crypto currency
		/// </summary>
		/// <param name="cryptoId">Id of the crypto currency</param>
		/// <returns>A dataset containing the previous prices of the currency</returns>
		[HttpGet("price/history/{CryptoId}")]
		public async Task<IActionResult> GetCryptoPriceHistoy(string cryptoId)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.StatusCode = 200;
				response.Data = await _unitOfWork.CryptoRepository.GetPriceHistory(cryptoId);
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
