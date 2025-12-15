using HrManager.Domain.Enums;

namespace HrManager.Application.DTOs.Responses;

public record EmployeeResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string DocumentNumber,
    DateTime BirthDate,
    EmployeeRole Role,
    Guid? ManagerId,
    string? ManagerName,
    IReadOnlyCollection<PhoneResponse> Phones,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
