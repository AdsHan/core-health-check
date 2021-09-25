using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheck.API.Checks
{
    public class ApiProductHealthCheck : IHealthCheck
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public ApiProductHealthCheck(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.GetAsync("http://api-product:5202");
                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy($"API is running!");
                }

                return HealthCheckResult.Unhealthy("API is not running!");
            }
        }
    }
}
