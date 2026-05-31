namespace LR_1.Clinic.Core;

public sealed class Frame
{
    public string Id { get; }
    public Concept Concept { get; }
    public IReadOnlyDictionary<string, object> Slots { get; }

    public Frame(string id, Concept concept, Dictionary<string, object>? slots = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(concept);

        Id = id;
        Concept = concept;
        Slots = slots ?? new Dictionary<string, object>();
    }

    public bool InstanceOf(Concept concept) => Concept.IsA(concept);

    public override string ToString() => Id;
}
