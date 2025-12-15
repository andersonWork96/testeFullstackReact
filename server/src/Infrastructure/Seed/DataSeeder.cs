using HrManager.Application.Abstractions.Services;
using HrManager.Domain.Entities;
using HrManager.Domain.Enums;
using HrManager.Domain.ValueObjects;
using HrManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HrManager.Infrastructure.Seed;

public static class DataSeeder
{
    public static async Task EnsureSeededAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;
        var context = provider.GetRequiredService<ApplicationDbContext>();
        var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("DataSeeder");
        var passwordHasher = provider.GetRequiredService<IPasswordHasher>();
        var configuration = provider.GetRequiredService<IConfiguration>();

        // Cria o schema. Para SQLite local, recriamos o arquivo para garantir as tabelas.
        var isSqlite = context.Database.ProviderName?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) == true;
        if (isSqlite)
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
        else
        {
            await context.Database.EnsureCreatedAsync();
        }

        if (await context.Employees.AnyAsync())
        {
            return;
        }

        var email = configuration.GetValue<string>("Seed:AdminEmail") ?? "admin@empresa.com";
        var password = configuration.GetValue<string>("Seed:AdminPassword") ?? "Trocar@123";

        var director = new Employee(
            "Admin",
            "Diretor",
            email,
            "00000000000",
            new DateTime(1990, 1, 1),
            EmployeeRole.Director,
            new List<PhoneNumber> { new("Principal", "+55 11 99999-9999") },
            passwordHasher.HashPassword(password));

        await context.Employees.AddAsync(director);
        await context.SaveChangesAsync();

        logger.LogInformation("Usuário diretor seed criado com e-mail {Email}", email);
    }
}
