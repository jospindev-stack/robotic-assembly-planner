using AssemblyPlanner.Domain;

namespace AssemblyPlanner.Engine;

public sealed class SequenceOptimizer
{
    public IReadOnlyList<Part> Optimize(
        Point2D start,
        IReadOnlyCollection<Part> parts,
        Func<Point2D, Point2D, double>? travelCost = null)
    {
        travelCost ??= Geometry.Distance;
        var initial = OptimizeNearestNeighbor(start, parts, travelCost);
        return ImproveTwoOpt(start, initial, travelCost);
    }

    public IReadOnlyList<Part> OptimizeNearestNeighbor(
        Point2D start,
        IReadOnlyCollection<Part> parts,
        Func<Point2D, Point2D, double>? travelCost = null)
    {
        travelCost ??= Geometry.Distance;
        var remaining = parts.ToList();
        var ordered = new List<Part>(remaining.Count);
        var current = start;

        while (remaining.Count > 0)
        {
            var next = remaining
                .OrderBy(part => travelCost(current, part.Position))
                .ThenBy(part => part.Id, StringComparer.Ordinal)
                .First();

            ordered.Add(next);
            remaining.Remove(next);
            current = next.Position;
        }

        return ordered;
    }

    public IReadOnlyList<Part> ImproveTwoOpt(
        Point2D start,
        IReadOnlyList<Part> route,
        Func<Point2D, Point2D, double>? travelCost = null)
    {
        travelCost ??= Geometry.Distance;

        if (route.Count < 3)
            return route.ToArray();

        var best = route.ToList();
        var bestDistance = CalculateDistance(start, best, travelCost);
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

                    var candidateDistance = CalculateDistance(start, candidate, travelCost);
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

    public double CalculateDistance(
        Point2D start,
        IReadOnlyList<Part> route,
        Func<Point2D, Point2D, double>? travelCost = null)
    {
        travelCost ??= Geometry.Distance;
        var current = start;
        var total = 0d;

        foreach (var part in route)
        {
            total += travelCost(current, part.Position);
            current = part.Position;
        }

        return total;
    }
}
