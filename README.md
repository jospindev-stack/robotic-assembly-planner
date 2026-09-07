# Robotic Assembly Planner

A C#/.NET 8 simulation project for industrial robotic assembly planning.

The application models assembly parts and restricted zones, proposes an optimized assembly sequence, computes collision-free robot travel paths, visualizes the work cell in 2D, and estimates total travel distance and cycle time. The project focuses on software engineering, computational geometry, optimization, automated testing, and industrial problem solving.

## Why this project

Manufacturing software often has to transform physical constraints into reliable software decisions. This project explores that problem through a simplified robotic work-cell planner.

The current version demonstrates:

- C# and .NET 8
- domain separation between models, planning engine, API, frontend, and tests
- 2D geometry
- nearest-neighbor sequence planning
- 2-opt sequence improvement
- obstacle-aware sequence cost evaluation
- rectangular obstacle collision detection
- visibility-graph path routing
- Dijkstra shortest-path search
- configurable safety clearance around obstacles
- total routed distance calculation
- cycle-time estimation
- ASP.NET Core REST API
- React/Vite 2D visualization
- animated robot movement along the calculated route
- xUnit automated tests
- GitHub Actions CI for backend, frontend, and Docker builds
- Docker and Docker Compose support

## Architecture

```text
src/
├── AssemblyPlanner.Domain/    # Parts, points, obstacles, path/result models
├── AssemblyPlanner.Engine/    # Geometry, routing, optimization and planning logic
└── AssemblyPlanner.Api/       # ASP.NET Core API

frontend/                      # React/Vite 2D work-cell visualization

tests/
└── AssemblyPlanner.Tests/     # Unit tests
```

## Planning flow

```text
Assembly request
      ↓
Nearest-neighbor initial sequence
      ↓
2-opt sequence improvement
      ↓
Obstacle-aware routed cost evaluation
      ↓
Visibility graph + Dijkstra routing
      ↓
Distance + cycle-time estimation
      ↓
2D animated plan visualization
```

A part that appears geometrically close can be deprioritized when restricted zones make the actual robot route significantly longer.

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
  "robotSpeedMmPerSecond": 120,
  "handlingTimeSeconds": 1.5
}
```

The response includes the selected assembly sequence, collision-free path segments, total routed distance, estimated cycle time, and collision count.

## Run with Docker

The complete stack can be started with one command:

```bash
docker compose up --build
```

Then open:

- Frontend: `http://localhost:5173`
- API health endpoint: `http://localhost:5000/health`

## Run locally without Docker

Backend:

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/AssemblyPlanner.Api
```

Frontend:

```bash
cd frontend
npm install
npm run dev
```

## CI

GitHub Actions validates:

1. .NET restore
2. .NET release build
3. xUnit tests with code coverage collection
4. frontend dependency installation
5. React production build
6. Docker Compose image build

## Roadmap

- configurable robot acceleration/deceleration constraints
- turn penalties and operation-time constraints
- benchmark different sequence and routing strategies
- optional A* grid planner for comparison with visibility-graph routing
- editable work-cell inputs from the frontend

## Disclaimer

This project is a portfolio simulation using synthetic data and simplified robot-motion assumptions. It is not intended to control real industrial equipment.
