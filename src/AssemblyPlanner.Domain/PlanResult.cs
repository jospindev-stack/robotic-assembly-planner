namespace AssemblyPlanner.Domain;

public sealed record PlanResult(
    IReadOnlyList<string> Sequence,
    IReadOnlyList<PathSegment> Segments,
    double TotalDistanceMm,
    double EstimatedCycleTimeSeconds,
    int CollisionCount);
