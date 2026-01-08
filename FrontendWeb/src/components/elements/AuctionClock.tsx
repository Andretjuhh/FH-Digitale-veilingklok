import React, {useEffect, useMemo, useState} from 'react';

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
	maxPrice?: number;
	minPrice?: number;
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
		maxPrice,
		minPrice,
		onTick,
		onComplete,
	} = props;

	const DOTS = 100; // number of dots around the ring
	const totalMs = totalSeconds * 1000;
	const [remainingMs, setRemainingMs] = useState<number>(totalMs);
	const [roundCount, setRoundCount] = useState<number>(round);
	const [isComplete, setIsComplete] = useState(false);

	useEffect(() => {
		setRoundCount(round);
	}, [round]);

	useEffect(() => {
		if (!start) return;
		setRemainingMs(totalMs);
		setIsComplete(false);
	}, [totalMs, start]);

	// External reset trigger
	useEffect(() => {
		if (!start) return;
		setRemainingMs(totalMs);
		setIsComplete(false);
	}, [props.resetToken]);

	useEffect(() => {
		if (!start || props.paused || isComplete) return;
		const id = window.setInterval(() => {
			setRemainingMs((r) => {
				if (r <= 0) return 0;
				const next = Math.max(0, r - 100);
				const secs = Math.ceil(next / 1000);
				onTick?.(secs);
				if (next === 0) {
					setIsComplete(true);
					onComplete?.();
					return 0;
				}
				return next;
			});
		}, 100);
		return () => window.clearInterval(id);
	}, [start, props.paused, totalMs, onTick, onComplete, isComplete]);

	const displayPrice = useMemo(() => {
		if (typeof maxPrice === 'number' && typeof minPrice === 'number') {
			const high = Math.max(maxPrice, minPrice);
			const low = Math.min(maxPrice, minPrice);
			if (high === low || totalMs === 0) return high;
			const progress = (totalMs - remainingMs) / totalMs;
			return high - (high - low) * progress;
		}
		return price;
	}, [maxPrice, minPrice, price, remainingMs, totalMs]);

	const ringHigh = 200;
	const ringLow = 0;

	const progressIndex = useMemo(() => {
		if (typeof maxPrice === 'number' && typeof minPrice === 'number') {
			const clamped = Math.min(ringHigh, Math.max(ringLow, displayPrice));
			const progress = (ringHigh - clamped) / (ringHigh - ringLow);
			return Math.min(DOTS - 1, Math.floor(progress * DOTS));
		}
		const elapsedMs = totalMs - remainingMs;
		return Math.min(DOTS - 1, Math.floor((elapsedMs / totalMs) * DOTS));
	}, [displayPrice, maxPrice, minPrice, remainingMs, totalMs]);

	const ringLabels = useMemo(() => {
		const steps = 10;
		const stepValue = (ringHigh - ringLow) / steps;
		return new Array(steps + 1).fill(0).map((_, i) => ringHigh - stepValue * i);
	}, []);

	const formatRingLabel = (value: number) => {
		return Number.isInteger(value) ? value.toString() : value.toFixed(2);
	};

	// Precompute ring dots
	const dots = useMemo(() => {
		const r = 135; // spread outwards to create more inner space
		const cx = 150; // center x
		const cy = 150; // center y
		return new Array(DOTS).fill(0).map((_, i) => {
			const angle = (i / DOTS) * 2 * Math.PI - Math.PI / 2; // start from top
			const x = cx + Math.cos(angle) * r;
			const y = cy + Math.sin(angle) * r;
			return {x, y};
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
				{ringLabels.map((label, index) => {
					const percent = index / (ringLabels.length - 1);
					const i = Math.round(percent * DOTS);
					const r = 145;
					const cx = 150;
					const cy = 150;
					const a = (i / DOTS) * 2 * Math.PI - Math.PI / 2;
					const x = cx + Math.cos(a) * r;
					const y = cy + Math.sin(a) * r;
					return (
						<text
							key={label}
							x={x}
							y={y}
							className="clock-num"
							textAnchor="middle"
							dominantBaseline="middle"
						>
							{formatRingLabel(label)}
						</text>
					);
				})}

				{/* Center panel */}
				<g className="clock-center" transform="translate(150,150)">
					{/* top row */}
					<text x={-40} y={-75} className="clock-label" textAnchor="middle">Ronde</text>
					<rect x={-58} y={-70} width={36} height={22} rx={4} className="clock-box"/>
					<text x={-40} y={-55} className="clock-value" textAnchor="middle">{roundCount}</text>

					<text x={40} y={-75} className="clock-label" textAnchor="middle">Munt</text>
					<rect x={22} y={-70} width={36} height={22} rx={4} className="clock-box"/>
					<text x={40} y={-55} className="clock-value" textAnchor="middle">{coin}</text>

					{/* second row */}
					<text x={0} y={-35} className="clock-label" textAnchor="middle">Aant/ehd</text>
					<rect x={-23} y={-30} width={46} height={22} rx={4} className="clock-box"/>
					<text x={0} y={-15} className="clock-value" textAnchor="middle">{amountPerLot}</text>

					{/* third row */}
					<text x={-20} y={28} className="clock-label" textAnchor="end">Prijs</text>
					<rect x={-90} y={10} width={70} height={24} rx={6} className="clock-box"/>
					<text x={-55} y={28} className="clock-value" textAnchor="middle">€ {displayPrice.toFixed(2)}</text>

					<text x={58} y={28} className="clock-label" textAnchor="end">Min. aant.</text>
					<rect x={60} y={10} width={44} height={24} rx={6} className="clock-box"/>
					<text x={82} y={28} className="clock-value" textAnchor="middle">{minAmount}</text>

					{/* bottom label */}
				</g>
			</svg>
		</div>
	);
}
