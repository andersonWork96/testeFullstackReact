using HrManager.Application.Abstractions.Repositories;
using HrManager.Domain.Entities;
using HrManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EmployeeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await _dbContext.Employees.AddAsync(employee, cancellationToken);
    }

    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Employees
            .Include(e => e.Manager)
            .Include(e => e.Phones)
            .Include(e => e.TeamMembers)
            .AsTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Employees
            .Include(e => e.Phones)
            .Include(e => e.Manager)
            .AsTracking()
            .FirstOrDefaultAsync(e => e.Email == email, cancellationToken);
    }

    public async Task<Employee?> GetByDocumentAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Employees
            .Include(e => e.Phones)
            .Include(e => e.Manager)
            .AsTracking()
            .FirstOrDefaultAsync(e => e.DocumentNumber == documentNumber, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Employees
            .Include(e => e.Manager)
            .Include(e => e.Phones)
            .AsSplitQuery()
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task RemoveAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _dbContext.Employees.Remove(employee);
        await Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
