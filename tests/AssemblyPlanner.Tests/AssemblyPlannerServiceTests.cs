using AssemblyPlanner.Domain;
using AssemblyPlanner.Engine;

namespace AssemblyPlanner.Tests;

public sealed class AssemblyPlannerServiceTests
{
    [Fact]
    public void Plan_RoutesAroundObstacle_AndReturnsCollisionFreePlan()
    {
        var collisionDetector = new CollisionDetector();
        var service = new AssemblyPlannerService(
            new SequenceOptimizer(),
            new ObstacleRouter(collisionDetector));

        var parts = new[] { new Part("A", new Point2D(100, 0)) };
        var obstacles = new[] { new Obstacle("ZONE", 40, -5, 60, 5) };

        var result = service.Plan(new Point2D(0, 0), parts, obstacles, 100, 1.5);

        Assert.Equal(new[] { "A" }, result.Sequence);
        Assert.True(result.TotalDistanceMm > 100);
        Assert.True(result.EstimatedCycleTimeSeconds > 2.5);
        Assert.Equal(0, result.CollisionCount);
        Assert.True(result.Segments.Count > 1);

        foreach (var segment in result.Segments)
            Assert.False(collisionDetector.Intersects(segment.From, segment.To, obstacles[0]));
    }
}
