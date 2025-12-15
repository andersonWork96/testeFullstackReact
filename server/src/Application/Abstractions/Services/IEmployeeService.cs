using HrManager.Application.DTOs.Requests;
using HrManager.Application.DTOs.Responses;
using HrManager.Domain.Primitives;

namespace HrManager.Application.Abstractions.Services;

public interface IEmployeeService
{
    Task<Result<EmployeeResponse>> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default);
    Task<Result<EmployeeResponse>> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<EmployeeResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<EmployeeResponse>>> ListAsync(CancellationToken cancellationToken = default);
}
