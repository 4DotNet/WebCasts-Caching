using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using _4DNCaching.Redis.Api.Configuration;
using _4DNCaching.Redis.Api.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace _4DNCaching.Redis.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly ILogger<DownloadController> _logger;

        private IConnectionMultiplexer _connection;
        private IDatabase _database;
        private readonly string _connectionString;


        public DownloadController(
            IOptions<CacheConfiguration> cacheConfig,
            ILogger<DownloadController> logger)
        {
            _logger = logger;
            _connectionString = cacheConfig.Value.ConnectionString;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cacheKey = "downloaded-gist-response";
            var responseObject = await GetFromCacheOrDownloadAsync(cacheKey, DownloadContentAsync<DownloadedContent>);
            return Ok(responseObject);
        }

        private async Task<T> GetFromCacheOrDownloadAsync<T>(string cacheKey, Func<Task<T>> initializeFunction)
        {
            bool cacheAvailable;
            try
            {
                await Connect();
                var value = await _database.StringGetAsync(cacheKey);
                if (!string.IsNullOrEmpty(value))
                {
                    return JsonSerializer.Deserialize<T>(value);
                }

                cacheAvailable = true;
            }
            catch (Exception)
            {
                _logger.LogWarning("Oops, apparently your cache is down...");
                cacheAvailable = false;
            }

            var downloadedObject = await initializeFunction();
            var jsonValue = JsonSerializer.Serialize(downloadedObject);

            if (cacheAvailable)
            {
                await _database.StringSetAsync(cacheKey, jsonValue);
            }

            return downloadedObject;
        }

        private static async Task<T> DownloadContentAsync<T>()
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://gist.githubusercontent.com/nikneem/66060dad016cabd426166f282a946f80/raw/581fe051d1b84c6799eecbdca5496b69c474292c/4DotNet-WebCasts-Caching.json");

            var response = await httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent);
        }

        public async Task Connect()
        {
            _connection = await ConnectionMultiplexer.ConnectAsync(_connectionString);
            _database = _connection.GetDatabase();
        }


    }
}