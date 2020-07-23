using System.Net.Http;
using System.Threading.Tasks;
using _4DNCaching.InMemory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace _4DNCaching.InMemory.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        public DownloadController(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<IActionResult> Get()
        {
            var responseObject = new DownloadResponse();
            const string cacheKey = "downloaded-gist-response";

            var responseData = _cache.Get<string>(cacheKey);
            if (string.IsNullOrEmpty(responseData))
            {
                responseObject.Log = "Making a call to the actual resource.";
                var httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get,
                    "https://gist.githubusercontent.com/nikneem/66060dad016cabd426166f282a946f80/raw/581fe051d1b84c6799eecbdca5496b69c474292c/4DotNet-WebCasts-Caching.json");

                var response = await httpClient.SendAsync(request);
                 responseData = await response.Content.ReadAsStringAsync();

                _cache.Set(cacheKey, responseData);
            }
            else
            {
                responseObject.Log = "Got the value from cache.";
            }

            responseObject.ResponseData = responseData;
            return Ok(responseData);
        }
    }
}
