namespace AssemblyPlanner.Domain;

public sealed record PathSegment(Point2D From, Point2D To, string? TargetPartId, IReadOnlyList<string> CollidingObstacleIds)
{
    public bool HasCollision => CollidingObstacleIds.Count > 0;
}
