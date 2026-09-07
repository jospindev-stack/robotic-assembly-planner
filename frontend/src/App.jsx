import { useEffect, useMemo, useState } from 'react';

const initialRequest = {
  start: { x: 40, y: 40 },
  parts: [
    { id: 'A', position: { x: 160, y: 90 } },
    { id: 'B', position: { x: 480, y: 140 } },
    { id: 'C', position: { x: 300, y: 320 } },
    { id: 'D', position: { x: 620, y: 250 } }
  ],
  obstacles: [
    { id: 'restricted-zone', minX: 260, minY: 120, maxX: 390, maxY: 245 }
  ],
  robotSpeedMmPerSecond: 120,
  handlingTimeSeconds: 1.5
};

function App() {
  const [plan, setPlan] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [progress, setProgress] = useState(0);

  const bounds = useMemo(() => ({ width: 700, height: 400 }), []);

  useEffect(() => {
    if (!plan?.segments?.length) {
      setProgress(0);
      return;
    }

    setProgress(0);
    const durationMs = 5000;
    const startedAt = performance.now();
    let frameId;

    const tick = (now) => {
      const next = Math.min(1, (now - startedAt) / durationMs);
      setProgress(next);
      if (next < 1) frameId = requestAnimationFrame(tick);
    };

    frameId = requestAnimationFrame(tick);
    return () => cancelAnimationFrame(frameId);
  }, [plan]);

  const robotPosition = useMemo(() => {
    if (!plan?.segments?.length) return initialRequest.start;

    const lengths = plan.segments.map((segment) => Math.hypot(
      segment.to.x - segment.from.x,
      segment.to.y - segment.from.y
    ));
    const total = lengths.reduce((sum, value) => sum + value, 0);
    if (total <= 0) return plan.segments.at(-1).to;

    let remaining = total * progress;

    for (let i = 0; i < plan.segments.length; i += 1) {
      const segment = plan.segments[i];
      const length = lengths[i];
      if (remaining <= length) {
        const ratio = length === 0 ? 1 : remaining / length;
        return {
          x: segment.from.x + (segment.to.x - segment.from.x) * ratio,
          y: segment.from.y + (segment.to.y - segment.from.y) * ratio
        };
      }
      remaining -= length;
    }

    return plan.segments.at(-1).to;
  }, [plan, progress]);

  async function generatePlan() {
    setLoading(true);
    setError('');

    try {
      const apiUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5000';
      const response = await fetch(`${apiUrl}/api/plans`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(initialRequest)
      });

      if (!response.ok) throw new Error(`API returned ${response.status}`);
      setPlan(await response.json());
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="page-shell">
      <section className="hero">
        <div>
          <p className="eyebrow">Industrial planning simulation</p>
          <h1>Robotic Assembly Planner</h1>
          <p className="lede">Obstacle-aware sequence optimization and collision-free path routing for a simplified robotic work cell.</p>
        </div>
        <button onClick={generatePlan} disabled={loading}>{loading ? 'Planning…' : 'Generate plan'}</button>
      </section>

      <section className="metrics">
        <article><span>Sequence</span><strong>{plan?.sequence?.join(' → ') ?? '—'}</strong></article>
        <article><span>Distance</span><strong>{plan ? `${plan.totalDistanceMm.toFixed(1)} mm` : '—'}</strong></article>
        <article><span>Cycle time</span><strong>{plan ? `${plan.estimatedCycleTimeSeconds.toFixed(2)} s` : '—'}</strong></article>
        <article><span>Collisions</span><strong>{plan?.collisionCount ?? '—'}</strong></article>
      </section>

      <section className="workspace-card">
        <div className="workspace-header">
          <div>
            <h2>2D work cell</h2>
            <p>Parts, restricted zones and calculated robot travel path.</p>
          </div>
          <div className="legend"><span className="dot part-dot" />Part <span className="dot path-dot" />Path <span className="zone-swatch" />Restricted zone</div>
        </div>

        <svg className="workspace" viewBox={`0 0 ${bounds.width} ${bounds.height}`} role="img" aria-label="Robotic work cell visualization">
          <rect x="0" y="0" width={bounds.width} height={bounds.height} className="cell-bg" />

          {initialRequest.obstacles.map((o) => (
            <g key={o.id}>
              <rect x={o.minX} y={o.minY} width={o.maxX - o.minX} height={o.maxY - o.minY} className="obstacle" />
              <text x={(o.minX + o.maxX) / 2} y={(o.minY + o.maxY) / 2} textAnchor="middle" className="obstacle-label">{o.id}</text>
            </g>
          ))}

          {plan?.segments?.map((segment, index) => (
            <line key={`${segment.from.x}-${segment.from.y}-${index}`} x1={segment.from.x} y1={segment.from.y} x2={segment.to.x} y2={segment.to.y} className="route-line" />
          ))}

          <circle cx={initialRequest.start.x} cy={initialRequest.start.y} r="8" className="start-point" />
          <text x={initialRequest.start.x + 12} y={initialRequest.start.y - 10} className="point-label">Start</text>

          {initialRequest.parts.map((part) => (
            <g key={part.id}>
              <circle cx={part.position.x} cy={part.position.y} r="9" className="part-point" />
              <text x={part.position.x + 13} y={part.position.y - 10} className="point-label">{part.id}</text>
            </g>
          ))}

          {plan && (
            <g className="robot-marker">
              <circle cx={robotPosition.x} cy={robotPosition.y} r="11" />
              <circle cx={robotPosition.x} cy={robotPosition.y} r="4" className="robot-core" />
            </g>
          )}
        </svg>

        {plan && <div className="animation-status">Simulation progress: {(progress * 100).toFixed(0)}%</div>}
        {error && <p className="error">{error}. Make sure the ASP.NET Core API is running.</p>}
      </section>
    </main>
  );
}

export default App;
