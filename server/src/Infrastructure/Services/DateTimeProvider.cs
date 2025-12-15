using HrManager.Application.Abstractions.Services;

namespace HrManager.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
