using HrManager.Application.DTOs.Requests;
using HrManager.Domain.Enums;
using HrManager.Domain.Primitives;

namespace HrManager.Application.Validators;

public static class EmployeeValidator
{
    private const int AdultAge = 18;

    public static Result ValidateCreate(CreateEmployeeRequest request, DateTime nowUtc)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            errors.Add("Nome e sobrenome são obrigatórios.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors.Add("E-mail é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.DocumentNumber))
        {
            errors.Add("Documento é obrigatório.");
        }

        if (request.BirthDate == default)
        {
            errors.Add("Data de nascimento é obrigatória.");
        }
        else if (CalculateAge(request.BirthDate, nowUtc) < AdultAge)
        {
            errors.Add("Funcionário precisa ser maior de idade.");
        }

        if (request.Phones is null || request.Phones.Count == 0)
        {
            errors.Add("Informe pelo menos um telefone.");
        }

        if (!Enum.IsDefined(request.Role))
        {
            errors.Add("Perfil inválido.");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            errors.Add("Senha precisa ter pelo menos 8 caracteres.");
        }

        if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
        {
            errors.Add("Senha e confirmação não conferem.");
        }

        return errors.Count == 0 ? Result.Success() : Result.Failure(errors.ToArray());
    }

    public static Result ValidateUpdate(UpdateEmployeeRequest request, DateTime nowUtc)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            errors.Add("Nome e sobrenome são obrigatórios.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors.Add("E-mail é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.DocumentNumber))
        {
            errors.Add("Documento é obrigatório.");
        }

        if (request.Phones is null || request.Phones.Count == 0)
        {
            errors.Add("Informe pelo menos um telefone.");
        }

        if (CalculateAge(request.BirthDate, nowUtc) < AdultAge)
        {
            errors.Add("Funcionário precisa ser maior de idade.");
        }

        if (request.Role is < EmployeeRole.Employee or > EmployeeRole.Director)
        {
            errors.Add("Perfil inválido.");
        }

        if (request.NewPassword is { Length: > 0 } && request.NewPassword.Length < 8)
        {
            errors.Add("Nova senha precisa ter pelo menos 8 caracteres.");
        }

        return errors.Count == 0 ? Result.Success() : Result.Failure(errors.ToArray());
    }

    public static int CalculateAge(DateTime birthDate, DateTime nowUtc)
    {
        var age = nowUtc.Year - birthDate.Year;
        if (birthDate.Date > nowUtc.AddYears(-age).Date)
        {
            age--;
        }

        return age;
    }
}
