using AssemblyPlanner.Domain;
using AssemblyPlanner.Engine;

namespace AssemblyPlanner.Tests;

public sealed class SequenceOptimizerTests
{
    [Fact]
    public void OptimizeNearestNeighbor_SelectsClosestPartFirst()
    {
        var parts = new[]
        {
            new Part("Far", new Point2D(100, 0)),
            new Part("Near", new Point2D(10, 0)),
            new Part("Middle", new Point2D(30, 0))
        };

        var result = new SequenceOptimizer().OptimizeNearestNeighbor(new Point2D(0, 0), parts);

        Assert.Equal(new[] { "Near", "Middle", "Far" }, result.Select(x => x.Id));
    }

    [Fact]
    public void ImproveTwoOpt_ReducesRouteDistanceWhenBetterOrderExists()
    {
        var optimizer = new SequenceOptimizer();
        var start = new Point2D(0, 0);
        var route = new[]
        {
            new Part("A", new Point2D(10, 0)),
            new Part("B", new Point2D(10, 10)),
            new Part("C", new Point2D(20, 0)),
            new Part("D", new Point2D(20, 10))
        };

        var before = optimizer.CalculateDistance(start, route);
        var improved = optimizer.ImproveTwoOpt(start, route);
        var after = optimizer.CalculateDistance(start, improved);

        Assert.True(after < before);
        Assert.Equal(route.Select(x => x.Id).OrderBy(x => x), improved.Select(x => x.Id).OrderBy(x => x));
    }
}
