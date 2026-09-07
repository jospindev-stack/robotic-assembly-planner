using AssemblyPlanner.Domain;
using AssemblyPlanner.Engine;

namespace AssemblyPlanner.Tests;

public sealed class GeometryTests
{
    [Fact]
    public void Distance_UsesPythagoreanDistance()
    {
        var result = Geometry.Distance(new Point2D(0, 0), new Point2D(3, 4));
        Assert.Equal(5, result, 6);
    }
}
