using Microsoft.AspNetCore.Mvc;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TranzactionsController : Controller
    {
		[HttpGet("{Id}")] //user id
		public async Task<IActionResult> GetUserTransactions() {
			//TODO: Implement get user transactions
			return null;
		}

		[HttpGet("{Id}")] //transaction id
		public async Task<IActionResult> GetTransationDetails() {
			//TODO: Implement get transaction details
			return null;
		}
	}
}
