using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;
public static class ServiceRegistrationCore
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        // ******** Distributed Cache **********
        //services.AddDistributedMemoryCache(); // In Memory
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:ConnectionString"];
        });
        Console.WriteLine($"***************** {configuration["Redis:ConnectionString"] } **********");
        return services;
    }
}