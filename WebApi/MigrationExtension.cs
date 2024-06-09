using DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

public static class MigrationExtension
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope serviceScope = app.ApplicationServices.CreateScope();
        using BaseDBContext  dBContext = serviceScope.ServiceProvider.GetRequiredService<BaseDBContext>();
        dBContext.Database.Migrate();
    }
}
