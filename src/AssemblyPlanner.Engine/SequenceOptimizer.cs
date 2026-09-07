using AssemblyPlanner.Domain;

namespace AssemblyPlanner.Engine;

public sealed class SequenceOptimizer
{
    public IReadOnlyList<Part> Optimize(Point2D start, IReadOnlyCollection<Part> parts)
    {
        var initial = OptimizeNearestNeighbor(start, parts);
        return ImproveTwoOpt(start, initial);
    }

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

    public IReadOnlyList<Part> ImproveTwoOpt(Point2D start, IReadOnlyList<Part> route)
    {
        if (route.Count < 3)
            return route.ToArray();

        var best = route.ToList();
        var bestDistance = CalculateDistance(start, best);
        var improved = true;

        while (improved)
        {
            improved = false;

            for (var i = 0; i < best.Count - 1; i++)
            {
                for (var k = i + 1; k < best.Count; k++)
                {
                    var candidate = best.ToList();
                    candidate.Reverse(i, k - i + 1);

                    var candidateDistance = CalculateDistance(start, candidate);
                    if (candidateDistance + 1e-9 >= bestDistance)
                        continue;

                    best = candidate;
                    bestDistance = candidateDistance;
                    improved = true;
                }
            }
        }

        return best;
    }

    public double CalculateDistance(Point2D start, IReadOnlyList<Part> route)
    {
        var current = start;
        var total = 0d;

        foreach (var part in route)
        {
            total += Geometry.Distance(current, part.Position);
            current = part.Position;
        }

        return total;
    }
}
