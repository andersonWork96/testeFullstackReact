using System.Security.Claims;
using HrManager.Application.Abstractions.Authentication;
using HrManager.Domain.Enums;

namespace HrManager.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var id = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? User?.FindFirstValue(ClaimTypes.Name);
            return Guid.TryParse(id, out var parsed) ? parsed : null;
        }
    }

    public EmployeeRole? Role
    {
        get
        {
            var role = User?.FindFirstValue(ClaimTypes.Role);
            return Enum.TryParse<EmployeeRole>(role, out var parsed) ? parsed : null;
        }
    }

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;
}
