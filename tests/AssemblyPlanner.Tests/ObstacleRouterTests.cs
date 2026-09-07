using AssemblyPlanner.Domain;
using AssemblyPlanner.Engine;

namespace AssemblyPlanner.Tests;

public sealed class ObstacleRouterTests
{
    private readonly CollisionDetector _collisionDetector = new();

    [Fact]
    public void FindShortestPath_ReturnsDirectPath_WhenNoObstacleBlocksRoute()
    {
        var router = new ObstacleRouter(_collisionDetector);
        var start = new Point2D(0, 0);
        var end = new Point2D(100, 0);

        var route = router.FindShortestPath(start, end, Array.Empty<Obstacle>());

        Assert.Equal(2, route.Count);
        Assert.Equal(start, route[0]);
        Assert.Equal(end, route[1]);
    }

    [Fact]
    public void FindShortestPath_AddsWaypoints_WhenObstacleBlocksDirectRoute()
    {
        var router = new ObstacleRouter(_collisionDetector);
        var start = new Point2D(0, 0);
        var end = new Point2D(100, 0);
        var obstacles = new[] { new Obstacle("O1", 40, -10, 60, 10) };

        var route = router.FindShortestPath(start, end, obstacles);

        Assert.True(route.Count > 2);
        Assert.Equal(start, route[0]);
        Assert.Equal(end, route[^1]);

        for (var i = 0; i < route.Count - 1; i++)
            Assert.False(_collisionDetector.Intersects(route[i], route[i + 1], obstacles[0]));
    }

    [Fact]
    public void FindShortestPath_ProducesLongerPath_WhenDetourIsRequired()
    {
        var router = new ObstacleRouter(_collisionDetector);
        var start = new Point2D(0, 0);
        var end = new Point2D(100, 0);
        var obstacles = new[] { new Obstacle("O1", 40, -10, 60, 10) };

        var route = router.FindShortestPath(start, end, obstacles);
        var routedDistance = Enumerable.Range(0, route.Count - 1)
            .Sum(i => Geometry.Distance(route[i], route[i + 1]));

        Assert.True(routedDistance > Geometry.Distance(start, end));
    }
}
