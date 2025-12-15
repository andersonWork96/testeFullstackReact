using HrManager.Application.Abstractions.Authentication;
using HrManager.Application.Abstractions.Repositories;
using HrManager.Application.Abstractions.Services;
using HrManager.Infrastructure.Authentication;
using HrManager.Infrastructure.Persistence;
using HrManager.Infrastructure.Repositories;
using HrManager.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HrManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        var connectionString = configuration.GetConnectionString("Default") ?? string.Empty;

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase))
            {
                options.UseNpgsql(connectionString);
            }
            else
            {
                options.UseSqlite(connectionString);
            }
        });

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
