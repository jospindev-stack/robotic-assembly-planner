using AssemblyPlanner.Domain;

namespace AssemblyPlanner.Engine;

public sealed class CollisionDetector
{
    public bool Intersects(Point2D from, Point2D to, Obstacle obstacle)
    {
        if (!obstacle.IsValid)
            throw new ArgumentException($"Obstacle '{obstacle.Id}' has invalid bounds.", nameof(obstacle));

        if (Contains(from, obstacle) || Contains(to, obstacle))
            return true;

        var bottomLeft = new Point2D(obstacle.MinX, obstacle.MinY);
        var bottomRight = new Point2D(obstacle.MaxX, obstacle.MinY);
        var topRight = new Point2D(obstacle.MaxX, obstacle.MaxY);
        var topLeft = new Point2D(obstacle.MinX, obstacle.MaxY);

        return SegmentsIntersect(from, to, bottomLeft, bottomRight)
            || SegmentsIntersect(from, to, bottomRight, topRight)
            || SegmentsIntersect(from, to, topRight, topLeft)
            || SegmentsIntersect(from, to, topLeft, bottomLeft);
    }

    private static bool Contains(Point2D point, Obstacle obstacle) =>
        point.X >= obstacle.MinX && point.X <= obstacle.MaxX
        && point.Y >= obstacle.MinY && point.Y <= obstacle.MaxY;

    private static bool SegmentsIntersect(Point2D p1, Point2D p2, Point2D q1, Point2D q2)
    {
        var o1 = Orientation(p1, p2, q1);
        var o2 = Orientation(p1, p2, q2);
        var o3 = Orientation(q1, q2, p1);
        var o4 = Orientation(q1, q2, p2);

        if (o1 != o2 && o3 != o4)
            return true;

        const double epsilon = 1e-9;
        if (Math.Abs(Cross(p1, p2, q1)) < epsilon && OnSegment(p1, q1, p2)) return true;
        if (Math.Abs(Cross(p1, p2, q2)) < epsilon && OnSegment(p1, q2, p2)) return true;
        if (Math.Abs(Cross(q1, q2, p1)) < epsilon && OnSegment(q1, p1, q2)) return true;
        if (Math.Abs(Cross(q1, q2, p2)) < epsilon && OnSegment(q1, p2, q2)) return true;

        return false;
    }

    private static int Orientation(Point2D a, Point2D b, Point2D c)
    {
        var cross = Cross(a, b, c);
        const double epsilon = 1e-9;
        if (Math.Abs(cross) < epsilon) return 0;
        return cross > 0 ? 1 : 2;
    }

    private static double Cross(Point2D a, Point2D b, Point2D c) =>
        (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);

    private static bool OnSegment(Point2D a, Point2D b, Point2D c) =>
        b.X <= Math.Max(a.X, c.X) && b.X >= Math.Min(a.X, c.X)
        && b.Y <= Math.Max(a.Y, c.Y) && b.Y >= Math.Min(a.Y, c.Y);
}
