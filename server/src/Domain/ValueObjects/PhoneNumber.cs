namespace HrManager.Domain.ValueObjects;

public sealed record PhoneNumber(string Label, string Number)
{
    public override string ToString() => $"{Label}:{Number}";
}
