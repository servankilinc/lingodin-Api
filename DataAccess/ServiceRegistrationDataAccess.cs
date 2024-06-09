using DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

public static class ServiceRegistrationDataAccess
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BaseDBContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("BaseDbConnectionString"));
        });
        return services;
    }
}