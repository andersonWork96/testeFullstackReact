using HrManager.Application.DTOs.Requests;
using HrManager.Application.DTOs.Responses;
using HrManager.Domain.Primitives;

namespace HrManager.Application.Abstractions.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
