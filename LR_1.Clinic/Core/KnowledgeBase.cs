namespace LR_1.Clinic.Core;

public sealed class KnowledgeBase
{
    private readonly List<Concept> _concepts = [];
    private readonly List<Frame> _frames = [];

    public IReadOnlyList<Concept> Concepts => _concepts;
    public IReadOnlyList<Frame> Frames => _frames;

    public Concept RegisterConcept(string name, Concept? parent = null)
    {
        var concept = new Concept(name, parent);
        _concepts.Add(concept);
        return concept;
    }

    public Frame RegisterFrame(string id, Concept concept, Dictionary<string, object>? slots = null)
    {
        var frame = new Frame(id, concept, slots);
        _frames.Add(frame);
        return frame;
    }

    public static bool InstanceOf(Frame frame, Concept concept)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(concept);
        return frame.InstanceOf(concept);
    }
}
