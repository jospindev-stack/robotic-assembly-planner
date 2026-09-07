using AssemblyPlanner.Domain;
using AssemblyPlanner.Engine;

namespace AssemblyPlanner.Tests;

public sealed class AssemblyPlannerServiceTests
{
    [Fact]
    public void Plan_ReturnsDistanceCycleTimeAndCollisionCount()
    {
        var service = new AssemblyPlannerService(new SequenceOptimizer(), new CollisionDetector());
        var parts = new[] { new Part("A", new Point2D(100, 0)) };
        var obstacles = new[] { new Obstacle("ZONE", 40, -5, 60, 5) };

        var result = service.Plan(new Point2D(0, 0), parts, obstacles, 100, 1.5);

        Assert.Equal(new[] { "A" }, result.Sequence);
        Assert.Equal(100, result.TotalDistanceMm, 6);
        Assert.Equal(2.5, result.EstimatedCycleTimeSeconds, 6);
        Assert.Equal(1, result.CollisionCount);
    }
}
