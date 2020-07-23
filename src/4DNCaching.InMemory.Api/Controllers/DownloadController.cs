using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace _4DNCaching.InMemory.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly IMemoryCache _cache;


        public async Task<IActionResult> Get()
        {
            // Get from cache
            // If not in cache -> Download stuff
            // Add cache
            // Response
            await Task.CompletedTask;
            return Ok();
        }


        public DownloadController(IMemoryCache cache)
        {
            _cache = cache;
        }

    }
}
