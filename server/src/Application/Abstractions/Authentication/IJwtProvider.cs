using HrManager.Domain.Entities;

namespace HrManager.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    (string token, DateTime expiresAt) Create(Employee employee);
}
