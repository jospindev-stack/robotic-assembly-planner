using AssemblyPlanner.Domain;

namespace AssemblyPlanner.Engine;

public sealed class SequenceOptimizer
{
    public IReadOnlyList<Part> OptimizeNearestNeighbor(Point2D start, IReadOnlyCollection<Part> parts)
    {
        var remaining = parts.ToList();
        var ordered = new List<Part>(remaining.Count);
        var current = start;

        while (remaining.Count > 0)
        {
            var next = remaining
                .OrderBy(part => Geometry.Distance(current, part.Position))
                .ThenBy(part => part.Id, StringComparer.Ordinal)
                .First();

            ordered.Add(next);
            remaining.Remove(next);
            current = next.Position;
        }

        return ordered;
    }
}
