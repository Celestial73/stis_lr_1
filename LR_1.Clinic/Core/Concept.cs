namespace LR_1.Clinic.Core;

public sealed class Concept
{
    public string Name { get; }
    public Concept? Parent { get; }

    public Concept(string name, Concept? parent = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        Parent = parent;
    }

    public bool IsA(Concept other)
    {
        ArgumentNullException.ThrowIfNull(other);

        for (Concept? current = this; current is not null; current = current.Parent)
        {
            if (ReferenceEquals(current, other))
                return true;
        }

        return false;
    }

    public override string ToString() => Name;
}
