using AssemblyPlanner.Domain;

namespace AssemblyPlanner.Engine;

public sealed class ObstacleRouter
{
    private readonly CollisionDetector _collisionDetector;
    private const double ClearanceMm = 5d;

    public ObstacleRouter(CollisionDetector collisionDetector)
    {
        _collisionDetector = collisionDetector;
    }

    public IReadOnlyList<Point2D> FindShortestPath(
        Point2D start,
        Point2D end,
        IReadOnlyCollection<Obstacle> obstacles)
    {
        if (IsVisible(start, end, obstacles))
            return new[] { start, end };

        var nodes = new List<Point2D> { start, end };

        foreach (var obstacle in obstacles)
        {
            if (!obstacle.IsValid)
                throw new ArgumentException($"Obstacle '{obstacle.Id}' has invalid bounds.", nameof(obstacles));

            nodes.Add(new Point2D(obstacle.MinX - ClearanceMm, obstacle.MinY - ClearanceMm));
            nodes.Add(new Point2D(obstacle.MinX - ClearanceMm, obstacle.MaxY + ClearanceMm));
            nodes.Add(new Point2D(obstacle.MaxX + ClearanceMm, obstacle.MinY - ClearanceMm));
            nodes.Add(new Point2D(obstacle.MaxX + ClearanceMm, obstacle.MaxY + ClearanceMm));
        }

        var distances = Enumerable.Repeat(double.PositiveInfinity, nodes.Count).ToArray();
        var previous = Enumerable.Repeat(-1, nodes.Count).ToArray();
        var visited = new bool[nodes.Count];
        distances[0] = 0d;

        for (var iteration = 0; iteration < nodes.Count; iteration++)
        {
            var current = -1;
            var bestDistance = double.PositiveInfinity;

            for (var i = 0; i < nodes.Count; i++)
            {
                if (!visited[i] && distances[i] < bestDistance)
                {
                    current = i;
                    bestDistance = distances[i];
                }
            }

            if (current < 0)
                break;

            if (current == 1)
                break;

            visited[current] = true;

            for (var candidate = 0; candidate < nodes.Count; candidate++)
            {
                if (candidate == current || visited[candidate])
                    continue;

                if (!IsVisible(nodes[current], nodes[candidate], obstacles))
                    continue;

                var tentative = distances[current] + Geometry.Distance(nodes[current], nodes[candidate]);
                if (tentative < distances[candidate])
                {
                    distances[candidate] = tentative;
                    previous[candidate] = current;
                }
            }
        }

        if (double.IsPositiveInfinity(distances[1]))
            throw new InvalidOperationException("No collision-free route could be found between the requested points.");

        var route = new List<Point2D>();
        for (var index = 1; index >= 0; index = previous[index])
        {
            route.Add(nodes[index]);
            if (index == 0)
                break;
        }

        route.Reverse();
        return route;
    }

    private bool IsVisible(Point2D from, Point2D to, IReadOnlyCollection<Obstacle> obstacles) =>
        obstacles.All(obstacle => !_collisionDetector.Intersects(from, to, obstacle));
}
