namespace LR_1.Clinic.Core;

public sealed class Relation
{
    public string Name { get; }
    public Relation? Parent { get; }
    public IReadOnlyList<Concept> ArgumentTypes { get; }

    public Relation(string name, Relation? parent = null, params Concept[] argumentTypes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        Parent = parent;
        ArgumentTypes = argumentTypes;
    }

    public bool IsA(Relation other)
    {
        ArgumentNullException.ThrowIfNull(other);

        for (Relation? current = this; current is not null; current = current.Parent)
        {
            if (ReferenceEquals(current, other))
                return true;
        }

        return false;
    }

    public void ValidateArguments(params Frame[] arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        if (ArgumentTypes.Count == 0)
            return;

        if (arguments.Length != ArgumentTypes.Count)
        {
            throw new ArgumentException(
                $"Связь «{Name}» ожидает {ArgumentTypes.Count} аргумент(ов), получено {arguments.Length}.");
        }

        for (var i = 0; i < ArgumentTypes.Count; i++)
        {
            if (arguments[i].InstanceOf(ArgumentTypes[i]))
                continue;

            throw new ArgumentException(
                $"Аргумент {i + 1} связи «{Name}» должен быть instance-of «{ArgumentTypes[i].Name}», " +
                $"получен «{arguments[i].Id}» ({arguments[i].Concept.Name}).");
        }
    }

    public bool TryValidateArguments(Frame[] arguments, out string? error)
    {
        try
        {
            ValidateArguments(arguments);
            error = null;
            return true;
        }
        catch (ArgumentException ex)
        {
            error = ex.Message;
            return false;
        }
    }

    public string FormatSignature()
    {
        if (ArgumentTypes.Count == 0)
            return $"{Name}(...)";

        var types = string.Join(", ", ArgumentTypes.Select(t => t.Name));
        return $"{Name}({types})";
    }

    public override string ToString() => Name;
}
