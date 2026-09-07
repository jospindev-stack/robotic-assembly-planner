namespace AssemblyPlanner.Domain;

public sealed record Obstacle(string Id, double MinX, double MinY, double MaxX, double MaxY)
{
    public bool IsValid => MinX <= MaxX && MinY <= MaxY;
}
