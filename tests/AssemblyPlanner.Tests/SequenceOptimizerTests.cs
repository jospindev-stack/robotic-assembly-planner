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
}
