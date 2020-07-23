using System.Net.Http;
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
            var cacheKey = "downloaded-gist-response";
            var responseData = _cache.Get<string>(cacheKey);
            if (string.IsNullOrEmpty(responseData))
            {
                var httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get,
                    "https://gist.githubusercontent.com/nikneem/66060dad016cabd426166f282a946f80/raw/581fe051d1b84c6799eecbdca5496b69c474292c/4DotNet-WebCasts-Caching.json");

                var response = await httpClient.SendAsync(request);
                 responseData = await response.Content.ReadAsStringAsync();

                _cache.Set(cacheKey, responseData);
            }
            return Ok(responseData);
        }


        public DownloadController(IMemoryCache cache)
        {
            _cache = cache;
        }

    }
}
