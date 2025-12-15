using HrManager.Domain.Enums;

namespace HrManager.Application.Abstractions.Authentication;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    EmployeeRole? Role { get; }
    bool IsAuthenticated { get; }
}
