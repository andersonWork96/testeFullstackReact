using HrManager.Application.Abstractions.Services;
using HrManager.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HrManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
