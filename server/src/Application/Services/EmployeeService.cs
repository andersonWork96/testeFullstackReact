using HrManager.Application.Abstractions.Authentication;
using HrManager.Application.Abstractions.Repositories;
using HrManager.Application.Abstractions.Services;
using HrManager.Application.DTOs.Requests;
using HrManager.Application.DTOs.Responses;
using HrManager.Application.Validators;
using HrManager.Domain.Entities;
using HrManager.Domain.Enums;
using HrManager.Domain.Primitives;
using HrManager.Domain.ValueObjects;

namespace HrManager.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _clock;

    public EmployeeService(
        IEmployeeRepository repository,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUser,
        IDateTimeProvider clock)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<Result<EmployeeResponse>> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var validation = EmployeeValidator.ValidateCreate(request, _clock.UtcNow);
        if (validation.IsFailure)
        {
            return Result.Failure<EmployeeResponse>(validation.Errors);
        }

        if (!_currentUser.IsAuthenticated)
        {
            return Result.Failure<EmployeeResponse>("Usuário não autenticado.");
        }

        if (!CanManage(request.Role))
        {
            return Result.Failure<EmployeeResponse>("Não é possível criar usuário com permissão superior à sua.");
        }

        if (await _repository.GetByEmailAsync(request.Email, cancellationToken) is not null)
        {
            return Result.Failure<EmployeeResponse>("E-mail já cadastrado.");
        }

        if (await _repository.GetByDocumentAsync(request.DocumentNumber, cancellationToken) is not null)
        {
            return Result.Failure<EmployeeResponse>("Documento já cadastrado.");
        }

        Employee? manager = null;
        if (request.ManagerId is Guid managerId)
        {
            manager = await _repository.GetByIdAsync(managerId, cancellationToken);
            if (manager is null)
            {
                return Result.Failure<EmployeeResponse>("Gestor informado não encontrado.");
            }

            if (manager.Role < request.Role)
            {
                return Result.Failure<EmployeeResponse>("Gestor não pode ter permissão inferior ao subordinado.");
            }
        }

        var phones = request.Phones.Select(p => new PhoneNumber(p.Label, p.Number)).ToList();
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var employee = new Employee(
            request.FirstName,
            request.LastName,
            request.Email,
            request.DocumentNumber,
            request.BirthDate,
            request.Role,
            phones,
            passwordHash,
            request.ManagerId);

        await _repository.AddAsync(employee, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success(ToResponse(employee, manager));
    }

    public async Task<Result<EmployeeResponse>> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var validation = EmployeeValidator.ValidateUpdate(request, _clock.UtcNow);
        if (validation.IsFailure)
        {
            return Result.Failure<EmployeeResponse>(validation.Errors);
        }

        var employee = await _repository.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return Result.Failure<EmployeeResponse>("Funcionário não encontrado.");
        }

        if (!CanManage(request.Role))
        {
            return Result.Failure<EmployeeResponse>("Não é possível elevar permissão acima da sua.");
        }

        var existingByEmail = await _repository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingByEmail is not null && existingByEmail.Id != employee.Id)
        {
            return Result.Failure<EmployeeResponse>("E-mail já cadastrado.");
        }

        var existingByDocument = await _repository.GetByDocumentAsync(request.DocumentNumber, cancellationToken);
        if (existingByDocument is not null && existingByDocument.Id != employee.Id)
        {
            return Result.Failure<EmployeeResponse>("Documento já cadastrado.");
        }

        Employee? manager = null;
        if (request.ManagerId is Guid managerId)
        {
            manager = await _repository.GetByIdAsync(managerId, cancellationToken);
            if (manager is null)
            {
                return Result.Failure<EmployeeResponse>("Gestor informado não encontrado.");
            }

            if (manager.Role < request.Role)
            {
                return Result.Failure<EmployeeResponse>("Gestor não pode ter permissão inferior ao subordinado.");
            }
        }

        employee.UpdateNames(request.FirstName, request.LastName);
        employee.UpdateContact(request.Email, request.Phones.Select(p => new PhoneNumber(p.Label, p.Number)));
        employee.UpdateRole(request.Role, request.ManagerId);

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            employee.UpdatePassword(_passwordHasher.HashPassword(request.NewPassword));
        }

        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success(ToResponse(employee, manager));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _repository.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return Result.Failure("Funcionário não encontrado.");
        }

        employee.Deactivate();
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<EmployeeResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _repository.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return Result.Failure<EmployeeResponse>("Funcionário não encontrado.");
        }

        return Result.Success(ToResponse(employee, employee.Manager));
    }

    public async Task<Result<IReadOnlyCollection<EmployeeResponse>>> ListAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _repository.GetAllAsync(cancellationToken);
        var mapped = employees.Select(e => ToResponse(e, e.Manager)).ToList();
        return Result.Success<IReadOnlyCollection<EmployeeResponse>>(mapped);
    }

    private bool CanManage(EmployeeRole targetRole)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.Role is null)
        {
            return false;
        }

        return _currentUser.Role.Value >= targetRole;
    }

    private static EmployeeResponse ToResponse(Employee employee, Employee? manager)
    {
        var phones = employee.Phones.Select(p => new PhoneResponse(p.Label, p.Number)).ToList();

        return new EmployeeResponse(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.DocumentNumber,
            employee.BirthDate,
            employee.Role,
            employee.ManagerId,
            manager?.FullName,
            phones,
            employee.IsActive,
            employee.CreatedAt,
            employee.UpdatedAt);
    }
}
