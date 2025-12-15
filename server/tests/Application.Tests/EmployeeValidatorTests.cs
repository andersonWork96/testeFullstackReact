using FluentAssertions;
using HrManager.Application.DTOs.Requests;
using HrManager.Application.Validators;
using HrManager.Domain.Enums;
using Xunit;

namespace HrManager.Application.Tests;

public class EmployeeValidatorTests
{
    [Fact]
    public void ValidateCreate_Should_Fail_When_Minor()
    {
        var request = new CreateEmployeeRequest(
            "João",
            "Silva",
            "joao@empresa.com",
            "12345678900",
            DateTime.UtcNow.AddYears(-17),
            EmployeeRole.Employee,
            null,
            new List<PhoneRequest> { new("Principal", "11999999999") },
            "SenhaSegura123",
            "SenhaSegura123");

        var result = EmployeeValidator.ValidateCreate(request, DateTime.UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Contains("maior de idade"));
    }

    [Fact]
    public void ValidateCreate_Should_Pass_For_Adult()
    {
        var request = new CreateEmployeeRequest(
            "Maria",
            "Oliveira",
            "maria@empresa.com",
            "98765432100",
            DateTime.UtcNow.AddYears(-25),
            EmployeeRole.Employee,
            null,
            new List<PhoneRequest> { new("Principal", "11988887777") },
            "SenhaSegura123",
            "SenhaSegura123");

        var result = EmployeeValidator.ValidateCreate(request, DateTime.UtcNow);

        result.IsSuccess.Should().BeTrue();
    }
}
