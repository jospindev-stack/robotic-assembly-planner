using AssemblyPlanner.Domain;

namespace AssemblyPlanner.Engine;

public sealed class AssemblyPlannerService
{
    private readonly SequenceOptimizer _sequenceOptimizer;
    private readonly ObstacleRouter _obstacleRouter;

    public AssemblyPlannerService(SequenceOptimizer sequenceOptimizer, ObstacleRouter obstacleRouter)
    {
        _sequenceOptimizer = sequenceOptimizer;
        _obstacleRouter = obstacleRouter;
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

        double RoutedCost(Point2D from, Point2D to)
        {
            var route = _obstacleRouter.FindShortestPath(from, to, obstacles);
            var distance = 0d;

            for (var i = 0; i < route.Count - 1; i++)
                distance += Geometry.Distance(route[i], route[i + 1]);

            return distance;
        }

        var orderedParts = _sequenceOptimizer.Optimize(start, parts, RoutedCost);
        var segments = new List<PathSegment>();
        var sequence = new List<string>();
        var current = start;
        var totalDistance = 0d;

        foreach (var part in orderedParts)
        {
            var route = _obstacleRouter.FindShortestPath(current, part.Position, obstacles);

            for (var i = 0; i < route.Count - 1; i++)
            {
                var isFinalLeg = i == route.Count - 2;
                var from = route[i];
                var to = route[i + 1];

                segments.Add(new PathSegment(
                    from,
                    to,
                    isFinalLeg ? part.Id : null,
                    Array.Empty<string>()));

                totalDistance += Geometry.Distance(from, to);
            }

            sequence.Add(part.Id);
            current = part.Position;
        }

        var estimatedCycleTime = totalDistance / robotSpeedMmPerSecond + orderedParts.Count * handlingTimeSeconds;

        return new PlanResult(sequence, segments, totalDistance, estimatedCycleTime, 0);
    }
}
