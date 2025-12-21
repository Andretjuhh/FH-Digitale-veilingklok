import React, { useEffect, useMemo, useState } from 'react';

type ClockProps = {
  totalSeconds?: number;
  start?: boolean;
  paused?: boolean;
  resetToken?: number; // change this value to reset to start
  // Center values
  round?: number;
  coin?: number;
  amountPerLot?: number; // "Aant/ehd"
  minAmount?: number;
  price?: number; // dynamic price shown
  onTick?: (remaining: number) => void;
  onComplete?: () => void;
};

// Simple SVG clock that paints 100 dots around and highlights progress
export default function AuctionClock(props: ClockProps) {
  const {
    totalSeconds = 8,
    start = true,
    round = 1,
    coin = 1,
    amountPerLot = 150,
    minAmount = 1,
    price = 1,
    onTick,
    onComplete,
  } = props;

  const DOTS = 100; // number of dots around the ring
  const totalMs = totalSeconds * 1000;
  const [remainingMs, setRemainingMs] = useState<number>(totalMs);
  const [roundCount, setRoundCount] = useState<number>(round);

  useEffect(() => {
    setRoundCount(round);
  }, [round]);

  useEffect(() => {
    if (!start) return;
    setRemainingMs(totalMs);
  }, [totalMs, start]);

  // External reset trigger
  useEffect(() => {
    if (!start) return;
    setRemainingMs(totalMs);
  }, [props.resetToken]);

  useEffect(() => {
    if (!start || props.paused) return;
    const id = window.setInterval(() => {
      setRemainingMs((r) => {
        const next = Math.max(0, r - 100);
        const secs = Math.ceil(next / 1000);
        onTick?.(secs);
        if (next === 0) {
          setRoundCount((prev) => prev + 1);
          onComplete?.();
          return totalMs; // reset to start position
        }
        return next;
      });
    }, 100);
    return () => window.clearInterval(id);
  }, [start, props.paused, totalMs, onTick, onComplete]);

  const progressIndex = useMemo(() => {
    const elapsedMs = totalMs - remainingMs;
    return Math.min(DOTS - 1, Math.floor((elapsedMs / totalMs) * DOTS));
  }, [remainingMs, totalMs]);

  // Precompute ring dots
  const dots = useMemo(() => {
    const r = 135; // spread outwards to create more inner space
    const cx = 150; // center x
    const cy = 150; // center y
    return new Array(DOTS).fill(0).map((_, i) => {
      const angle = (i / DOTS) * 2 * Math.PI - Math.PI / 2; // start from top
      const x = cx + Math.cos(angle) * r;
      const y = cy + Math.sin(angle) * r;
      return { x, y };
    });
  }, []);

  return (
    <div className="auction-clock">
      <svg viewBox="0 0 300 300" className="clock-svg" aria-label="Auction clock">
        {/* Outer ticks (dots) */}
        {dots.map((d, i) => (
          <circle
            key={i}
            cx={d.x}
            cy={d.y}
            r={4}
            className={i === progressIndex ? 'clock-dot clock-dot--active' : 'clock-dot'}
          />
        ))}

        {/* Minute marks numbers every 10% */}
        {[0,10,20,30,40,50,60,70,80,90].map((n) => {
          const i = Math.round((n/100)*DOTS);
          const r = 145; const cx = 150; const cy = 150; const a = (i / DOTS) * 2 * Math.PI - Math.PI/2;
          const x = cx + Math.cos(a) * r; const y = cy + Math.sin(a) * r;
          return <text key={n} x={x} y={y} className="clock-num" textAnchor="middle" dominantBaseline="middle">{n === 0 ? 0 : n}</text>
        })}

        {/* Center panel */}
        <g className="clock-center" transform="translate(150,150)">
          {/* top row */}
          <text x={-40} y={-75} className="clock-label" textAnchor="middle">Ronde</text>
          <rect x={-58} y={-70} width={36} height={22} rx={4} className="clock-box" />
          <text x={-40} y={-55} className="clock-value" textAnchor="middle">{roundCount}</text>

          <text x={40} y={-75} className="clock-label" textAnchor="middle">Munt</text>
          <rect x={22} y={-70} width={36} height={22} rx={4} className="clock-box" />
          <text x={40} y={-55} className="clock-value" textAnchor="middle">{coin}</text>

          {/* second row */}
          <text x={0} y={-35} className="clock-label" textAnchor="middle">Aant/ehd</text>
          <rect x={-23} y={-30} width={46} height={22} rx={4} className="clock-box" />
          <text x={0} y={-15} className="clock-value" textAnchor="middle">{amountPerLot}</text>

          {/* third row */}
          <text x={-20} y={28} className="clock-label" textAnchor="end">Prijs</text>
          <rect x={-90} y={10} width={70} height={24} rx={6} className="clock-box" />
          <text x={-55} y={28} className="clock-value" textAnchor="middle">€ {price.toFixed(2)}</text>

          <text x={58} y={28} className="clock-label" textAnchor="end">Min. aant.</text>
          <rect x={60} y={10} width={44} height={24} rx={6} className="clock-box" />
          <text x={82} y={28} className="clock-value" textAnchor="middle">{minAmount}</text>

          {/* bottom label */}
        </g>
      </svg>
    </div>
  );
}
