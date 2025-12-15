using HrManager.Domain.Enums;

namespace HrManager.Application.DTOs.Requests;

public record CreateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    string DocumentNumber,
    DateTime BirthDate,
    EmployeeRole Role,
    Guid? ManagerId,
    IReadOnlyCollection<PhoneRequest> Phones,
    string Password,
    string ConfirmPassword);
