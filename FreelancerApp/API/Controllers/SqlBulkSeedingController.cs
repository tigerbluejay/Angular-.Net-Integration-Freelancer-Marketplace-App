using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("dev")]
	public class SqlBulkSeedingController : ControllerBase
	{
		private readonly BulkSeedService _bulkSeedService;

		public SqlBulkSeedingController(BulkSeedService bulkSeedService)
		{
			_bulkSeedService = bulkSeedService;
		}

		[HttpPost("bulk-seed-users")]
		public async Task<IActionResult> BulkSeedUsers()
		{
			await _bulkSeedService.SeedUsersAsync();

			return Ok();
		}
	}
}
