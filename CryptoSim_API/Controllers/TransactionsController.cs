using Microsoft.AspNetCore.Mvc;

namespace CryptoSim_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class TransactionsController : Controller
    {
		[HttpGet("{UserId}")] //user id
		public async Task<IActionResult> GetUserTransactions() {
			//TODO: Implement get user transactions
			return null;
		}

		[HttpGet("{TransationId}")] //transaction id
		public async Task<IActionResult> GetTransationDetails() {
			//TODO: Implement get transaction details
			return null;
		}
	}
}
