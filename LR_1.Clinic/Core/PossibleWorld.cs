namespace LR_1.Clinic.Core;

public sealed class PossibleWorld
{
    private readonly List<Fact> _facts = [];

    public string Name { get; }

    public IReadOnlyList<Fact> Facts => _facts;

    public PossibleWorld(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }

    public void AddFact(Relation relation, params Frame[] arguments)
    {
        ArgumentNullException.ThrowIfNull(relation);
        ArgumentNullException.ThrowIfNull(arguments);

        relation.ValidateArguments(arguments);
        _facts.Add(new Fact(relation.Name, arguments));
    }

    public bool HasFact(Relation relation, params Frame[] arguments)
    {
        ArgumentNullException.ThrowIfNull(relation);
        return HasFact(relation.Name, arguments);
    }

    public bool HasFact(string predicate, params Frame[] arguments)
    {
        return _facts.Any(f =>
            f.Predicate == predicate &&
            f.Arguments.Count == arguments.Length &&
            f.Arguments.Zip(arguments).All(pair => ReferenceEquals(pair.First, pair.Second)));
    }

    public override string ToString() => Name;
}
