using HrManager.Domain.Enums;
using HrManager.Domain.ValueObjects;

namespace HrManager.Domain.Entities;

public class Employee
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string DocumentNumber { get; private set; } = string.Empty;
    public DateTime BirthDate { get; private set; }
    public EmployeeRole Role { get; private set; } = EmployeeRole.Employee;
    public Guid? ManagerId { get; private set; }
    public Employee? Manager { get; private set; }
    public List<Employee> TeamMembers { get; private set; } = new();
    public List<PhoneNumber> Phones { get; private set; } = new();
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private Employee()
    {
    }

    public Employee(
        string firstName,
        string lastName,
        string email,
        string documentNumber,
        DateTime birthDate,
        EmployeeRole role,
        IEnumerable<PhoneNumber> phones,
        string passwordHash,
        Guid? managerId = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        DocumentNumber = documentNumber;
        BirthDate = birthDate;
        Role = role;
        Phones = phones.ToList();
        PasswordHash = passwordHash;
        ManagerId = managerId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public string FullName => $"{FirstName} {LastName}";

    public int Age => (int)((DateTime.UtcNow - BirthDate).TotalDays / 365.25);

    public void UpdateNames(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        Touch();
    }

    public void UpdateContact(string email, IEnumerable<PhoneNumber> phones)
    {
        Email = email;
        Phones = phones.ToList();
        Touch();
    }

    public void UpdateRole(EmployeeRole role, Guid? managerId)
    {
        Role = role;
        ManagerId = managerId;
        Touch();
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }

    private void Touch() => UpdatedAt = DateTime.UtcNow;
}
