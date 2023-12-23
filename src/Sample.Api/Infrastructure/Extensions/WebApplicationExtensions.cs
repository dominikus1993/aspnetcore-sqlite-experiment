using Microsoft.EntityFrameworkCore;

using Sample.Api.Core.Repositories;
using Sample.Api.Infrastructure.EntityFramework;
using Sample.Api.Infrastructure.Repositories;

namespace Sample.Api.Infrastructure.Extensions;

public sealed class ConnectionStrings
{
    public string DefaultConnection { get; set; } = null!;
}

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        var configSection = builder.Configuration.GetSection("ConnectionStrings");
        builder.Services.Configure<ConnectionStrings>(configSection); // 👈 Calls the source generator

        var cfg = configSection.Get<ConnectionStrings>();
        builder.Services.AddScoped<PersonsDataSeeder>();
        builder.Services.AddScoped<IPersonsWriter, EfCorePersonsWriter>();
        builder.Services.AddDbContextPool<SampleDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            optionsBuilder.UseSqlite(cfg!.DefaultConnection);
        });
        return builder;
    }

    public static async Task SetupDatabase(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<SampleDbContext>();
        await context.Database.EnsureCreatedAsync();
        await context.Database.MigrateAsync();
        var seeder = scope.ServiceProvider.GetRequiredService<PersonsDataSeeder>();
        var writer = scope.ServiceProvider.GetRequiredService<IPersonsWriter>();
        var result = seeder.Seed();
        await writer.AddMany(result);
    }
}