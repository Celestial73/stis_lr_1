namespace LR_1.Clinic.Core;

public sealed class KripkeScale
{
    private readonly List<PossibleWorld> _worlds = [];
    private readonly Dictionary<PossibleWorld, HashSet<PossibleWorld>> _accessibility = new();

    public IReadOnlyList<PossibleWorld> Worlds => _worlds;

    public PossibleWorld AddWorld(string name)
    {
        var world = new PossibleWorld(name);
        _worlds.Add(world);
        _accessibility.TryAdd(world, []);
        return world;
    }

    public void AddAccessibility(PossibleWorld from, PossibleWorld to)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);

        if (!_accessibility.ContainsKey(from))
            _accessibility[from] = [];

        _accessibility[from].Add(to);
    }

    public IEnumerable<PossibleWorld> ReachableFrom(PossibleWorld start)
    {
        ArgumentNullException.ThrowIfNull(start);

        var visited = new HashSet<PossibleWorld>();
        var queue = new Queue<PossibleWorld>();
        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            yield return current;

            if (!_accessibility.TryGetValue(current, out var nextWorlds))
                continue;

            foreach (var next in nextWorlds)
            {
                if (visited.Add(next))
                    queue.Enqueue(next);
            }
        }
    }

    public IEnumerable<PossibleWorld> GetAccessible(PossibleWorld from)
    {
        if (_accessibility.TryGetValue(from, out var nextWorlds))
            return nextWorlds;

        return [];
    }
}
