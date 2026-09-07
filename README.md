# Robotic Assembly Planner

A C#/.NET 8 simulation project for industrial robotic assembly planning.

The application models assembly parts and restricted zones, proposes an assembly sequence, detects path collisions, and estimates travel distance and cycle time. The project is intentionally focused on software engineering, geometry, optimization, automated testing, and industrial use cases.

## Why this project

Manufacturing software often has to transform physical constraints into reliable software decisions. This project explores that problem through a simplified robotic work-cell planner.

The first version demonstrates:

- C# and .NET 8
- domain-driven separation between models, planning engine, API, and tests
- 2D geometry
- nearest-neighbor sequence planning
- rectangular obstacle collision detection
- total path distance calculation
- cycle-time estimation
- ASP.NET Core REST API
- xUnit automated tests
- GitHub Actions CI

## Architecture

```text
src/
├── AssemblyPlanner.Domain/    # Parts, points, obstacles, path/result models
├── AssemblyPlanner.Engine/    # Geometry, collision and planning logic
└── AssemblyPlanner.Api/       # ASP.NET Core API

tests/
└── AssemblyPlanner.Tests/     # Unit tests
```

## Planning flow

```text
Assembly request
      ↓
Sequence optimizer
      ↓
Path generation
      ↓
Collision detection
      ↓
Distance + cycle-time estimation
      ↓
Plan result
```

## API

`POST /api/plans`

Example request:

```json
{
  "start": { "x": 0, "y": 0 },
  "parts": [
    { "id": "A", "position": { "x": 120, "y": 80 } },
    { "id": "B", "position": { "x": 400, "y": 120 } },
    { "id": "C", "position": { "x": 250, "y": 300 } }
  ],
  "obstacles": [
    { "id": "restricted-zone", "minX": 300, "minY": 150, "maxX": 380, "maxY": 260 }
  ],
  "robotSpeedMmPerSecond": 120
}
```

## Run locally

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/AssemblyPlanner.Api
```

## Roadmap

- obstacle-aware path routing rather than collision reporting only
- 2-opt sequence improvement
- A* path planning
- animated 2D work-cell visualization
- configurable robot acceleration and operation-time constraints
- Docker support
- benchmark different planning strategies

## Disclaimer

This project is a portfolio simulation using synthetic data and simplified robot-motion assumptions. It is not intended to control real industrial equipment.
