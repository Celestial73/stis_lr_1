namespace LR_1.Clinic.Core;

public sealed record Fact(string Predicate, IReadOnlyList<Frame> Arguments)
{
    public override string ToString()
    {
        var args = string.Join(", ", Arguments.Select(a => a.Id));
        return $"{Predicate}({args})";
    }
}
