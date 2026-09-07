using AssemblyPlanner.Domain;

namespace AssemblyPlanner.Engine;

public sealed class AssemblyPlannerService
{
    private readonly SequenceOptimizer _sequenceOptimizer;
    private readonly CollisionDetector _collisionDetector;

    public AssemblyPlannerService(SequenceOptimizer sequenceOptimizer, CollisionDetector collisionDetector)
    {
        _sequenceOptimizer = sequenceOptimizer;
        _collisionDetector = collisionDetector;
    }

    public PlanResult Plan(
        Point2D start,
        IReadOnlyCollection<Part> parts,
        IReadOnlyCollection<Obstacle> obstacles,
        double robotSpeedMmPerSecond,
        double handlingTimeSeconds)
    {
        if (robotSpeedMmPerSecond <= 0)
            throw new ArgumentOutOfRangeException(nameof(robotSpeedMmPerSecond), "Robot speed must be greater than zero.");
        if (handlingTimeSeconds < 0)
            throw new ArgumentOutOfRangeException(nameof(handlingTimeSeconds));

        var orderedParts = _sequenceOptimizer.OptimizeNearestNeighbor(start, parts);
        var segments = new List<PathSegment>();
        var sequence = new List<string>();
        var current = start;
        var totalDistance = 0d;
        var collisions = 0;

        foreach (var part in orderedParts)
        {
            var collisionIds = obstacles
                .Where(obstacle => _collisionDetector.Intersects(current, part.Position, obstacle))
                .Select(obstacle => obstacle.Id)
                .OrderBy(id => id, StringComparer.Ordinal)
                .ToArray();

            var segment = new PathSegment(current, part.Position, part.Id, collisionIds);
            segments.Add(segment);
            sequence.Add(part.Id);
            totalDistance += Geometry.Distance(current, part.Position);
            collisions += collisionIds.Length;
            current = part.Position;
        }

        var estimatedCycleTime = totalDistance / robotSpeedMmPerSecond + orderedParts.Count * handlingTimeSeconds;

        return new PlanResult(sequence, segments, totalDistance, estimatedCycleTime, collisions);
    }
}
