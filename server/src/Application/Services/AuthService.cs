using HrManager.Application.Abstractions.Authentication;
using HrManager.Application.Abstractions.Repositories;
using HrManager.Application.Abstractions.Services;
using HrManager.Application.DTOs.Requests;
using HrManager.Application.DTOs.Responses;
using HrManager.Domain.Primitives;

namespace HrManager.Application.Services;

public class AuthService : IAuthService
{
    private readonly IEmployeeRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(
        IEmployeeRepository repository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await _repository.GetByEmailAsync(request.Email, cancellationToken);
        if (employee is null)
        {
            return Result.Failure<AuthResponse>("Usuário ou senha inválidos.");
        }

        if (!_passwordHasher.Verify(employee.PasswordHash, request.Password))
        {
            return Result.Failure<AuthResponse>("Usuário ou senha inválidos.");
        }

        var (token, expiresAt) = _jwtProvider.Create(employee);

        var user = new EmployeeSummary(employee.Id, employee.FullName, employee.Email, employee.Role.ToString());
        return Result.Success(new AuthResponse(token, expiresAt, user));
    }
}
