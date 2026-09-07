using AssemblyPlanner.Domain;

namespace AssemblyPlanner.Engine;

public static class Geometry
{
    public static double Distance(Point2D a, Point2D b)
    {
        var dx = b.X - a.X;
        var dy = b.Y - a.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}
