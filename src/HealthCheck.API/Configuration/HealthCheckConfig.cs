using HealthCheck.API.Checks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCheck.API.Configuration
{
    public static class HealthCheckConfig
    {
        public static IServiceCollection AddHealthCheckConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck<InternetHealthCheck>(name: "Internet", tags: new[] { "infrastructure", "connection is ON" })
                .AddRedis(configuration.GetConnectionString("RedisCs"), name: "Redis", tags: new[] { "infrastructure", "cache" })
                .AddNpgSql(configuration.GetConnectionString("PostgreSQLCs"), name: "PostgreSQL", tags: new[] { "infrastructure", "data" })
                .AddElasticsearch(configuration.GetConnectionString("ElasticsearchCs"), name: "Elasticsearch", tags: new[] { "infrastructure", "logs" })
                .AddCheck<ApiProductHealthCheck>(name: "Api - Product", tags: new[] { "api" });

            services.AddHealthChecksUI().AddInMemoryStorage();

            return services;
        }
        public static IApplicationBuilder UseHealthCheckConfiguration(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/infrastructure", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("infrastructure"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/api", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("api"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecksUI(options =>
            {
                options.UIPath = "/status-ui";
                options.ApiPath = "/status-ui-api";
            });

            return app;
        }
    }
}