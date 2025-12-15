namespace HrManager.Application.DTOs.Responses;

public record AuthResponse(string Token, DateTime ExpiresAt, EmployeeSummary User);

public record EmployeeSummary(Guid Id, string FullName, string Email, string Role);
