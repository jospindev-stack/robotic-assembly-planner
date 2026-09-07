using AssemblyPlanner.Domain;
using AssemblyPlanner.Engine;

namespace AssemblyPlanner.Tests;

public sealed class CollisionDetectorTests
{
    private readonly CollisionDetector _detector = new();

    [Fact]
    public void Intersects_ReturnsTrue_WhenPathCrossesObstacle()
    {
        var obstacle = new Obstacle("O1", 4, -1, 6, 1);
        Assert.True(_detector.Intersects(new Point2D(0, 0), new Point2D(10, 0), obstacle));
    }

    [Fact]
    public void Intersects_ReturnsFalse_WhenPathMissesObstacle()
    {
        var obstacle = new Obstacle("O1", 4, 4, 6, 6);
        Assert.False(_detector.Intersects(new Point2D(0, 0), new Point2D(10, 0), obstacle));
    }
}
