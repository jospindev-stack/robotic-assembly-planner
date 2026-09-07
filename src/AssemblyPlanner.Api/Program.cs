using AssemblyPlanner.Domain;
using AssemblyPlanner.Engine;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<SequenceOptimizer>();
builder.Services.AddSingleton<CollisionDetector>();
builder.Services.AddSingleton<AssemblyPlannerService>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/api/plans", (PlanRequest request, AssemblyPlannerService planner) =>
{
    var parts = request.Parts
        .Select(p => new Part(p.Id, new Point2D(p.Position.X, p.Position.Y)))
        .ToArray();

    var obstacles = request.Obstacles
        .Select(o => new Obstacle(o.Id, o.MinX, o.MinY, o.MaxX, o.MaxY))
        .ToArray();

    var result = planner.Plan(
        new Point2D(request.Start.X, request.Start.Y),
        parts,
        obstacles,
        request.RobotSpeedMmPerSecond,
        request.HandlingTimeSeconds);

    return Results.Ok(result);
});

app.Run();

public sealed record PointRequest(double X, double Y);
public sealed record PartRequest(string Id, PointRequest Position);
public sealed record ObstacleRequest(string Id, double MinX, double MinY, double MaxX, double MaxY);
public sealed record PlanRequest(
    PointRequest Start,
    double RobotSpeedMmPerSecond,
    double HandlingTimeSeconds,
    IReadOnlyCollection<PartRequest> Parts,
    IReadOnlyCollection<ObstacleRequest> Obstacles);
