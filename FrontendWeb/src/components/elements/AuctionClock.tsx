import React, { forwardRef, useEffect, useImperativeHandle, useMemo, useState } from 'react';
import { motion } from 'framer-motion';
import { formatEur } from '../../utils/standards';
import { useRootContext } from '../contexts/RootContext';
import { VeilingPriceTickNotification } from '../../declarations/models/VeilingNotifications';

interface AuctionClockProps {
	// Clock configuration
	totalDots?: number;
	dotSize?: number;
	dotSpacing?: number; // radius of the ring

	// Clock data
	highestPrice?: number;
	lowestPrice?: number;
	currentPrice?: number;

	// Center display values
	round?: number;
	coin?: number;
	amountStock?: number;
	minAmount?: number;

	// Control
	autoStart?: boolean;
	paused?: boolean;

	// Callbacks
	onTick?: (price: number) => void;
	onComplete?: () => void;
}

export interface AuctionClockRef {
	tick: (state: VeilingPriceTickNotification) => void;
	reset: () => void;
	pause: () => void;
	resume: () => void;
}

const AuctionClock = forwardRef<AuctionClockRef, AuctionClockProps>((props, ref) => {
	const { totalDots = 100, dotSize = 3, dotSpacing = 135, highestPrice = 200, lowestPrice = 0, currentPrice: initialPrice, round = 1, coin = 1, amountStock = 150, minAmount = 1, autoStart = false, paused = false, onTick, onComplete } = props;

	const { t } = useRootContext();
	const [currentPrice, setCurrentPrice] = useState<number>(initialPrice ?? highestPrice);
	const [isPaused, setIsPaused] = useState(paused);
	const [isComplete, setIsComplete] = useState(false);

	// Calculate progress based on price (high to low)
	const progressIndex = useMemo(() => {
		const priceRange = highestPrice - lowestPrice;
		if (priceRange === 0) return 0;

		const priceDrop = highestPrice - currentPrice;
		const progress = priceDrop / priceRange;

		return Math.min(totalDots - 1, Math.floor(progress * totalDots));
	}, [currentPrice, highestPrice, lowestPrice, totalDots]);

	// Generate ring labels (price markers)
	const ringLabels = useMemo(() => {
		const steps = 10;
		const stepValue = (highestPrice - lowestPrice) / steps;
		return new Array(steps + 1).fill(0).map((_, i) => highestPrice - stepValue * i);
	}, [highestPrice, lowestPrice]);

	// Precompute dot positions (counter-clockwise, opposite of clock)
	const dots = useMemo(() => {
		const cx = 150;
		const cy = 150;
		return new Array(totalDots).fill(0).map((_, i) => {
			// Reverse direction: subtract instead of add angle for counter-clockwise
			const angle = -((i / totalDots) * 2 * Math.PI) - Math.PI / 2; // start from top, go counter-clockwise
			const x = cx + Math.cos(angle) * dotSpacing;
			const y = cy + Math.sin(angle) * dotSpacing;
			return { x, y };
		});
	}, [totalDots, dotSpacing]);

	// Expose methods via ref
	useImperativeHandle(
		ref,
		() => ({
			tick: (state: VeilingPriceTickNotification) => {
				setCurrentPrice(state.currentPrice);
				onTick?.(state.currentPrice);

				// Check if we've reached the lowest price
				if (state.currentPrice <= lowestPrice) {
					setIsComplete(true);
					onComplete?.();
				}
			},
			reset: () => {
				setCurrentPrice(highestPrice);
				setIsComplete(false);
				setIsPaused(false);
			},
			pause: () => setIsPaused(true),
			resume: () => setIsPaused(false),
		}),
		[highestPrice, lowestPrice, onTick, onComplete]
	);

	// Sync with paused prop
	useEffect(() => {
		setIsPaused(paused);
	}, [paused]);

	const formatPrice = (value: number) => {
		return Math.round(value).toString();
	};

	// Determine dot color based on position
	const getDotColor = (index: number) => {
		if (index === 0) return 'clock-dot clock-dot--highest'; // Green for highest
		if (index === totalDots - 1) return 'clock-dot clock-dot--lowest'; // Red for lowest
		if (index === progressIndex) return 'clock-dot clock-dot--active';
		return 'clock-dot';
	};

	return (
		<div className="auction-clock">
			<svg viewBox="0 0 300 300" className="clock-svg" aria-label="Auction clock">
				{/* Outer ring dots */}
				{dots.map((d, i) => {
					const isActive = i === progressIndex;
					const isHighest = i === 0;
					const isLowest = i === totalDots - 1;

					return (
						<motion.circle
							key={i}
							cx={d.x}
							cy={d.y}
							r={dotSize}
							className={getDotColor(i)}
							initial={false}
							animate={{
								scale: 1,
								opacity: isActive ? 1 : isHighest || isLowest ? 0.9 : 0.6,
							}}
							transition={{
								duration: 0.4,
								ease: 'easeInOut',
							}}
						/>
					);
				})}

				{/* Price labels around the ring */}
				{ringLabels.map((label, index) => {
					const percent = index / (ringLabels.length - 1);
					const i = Math.round(percent * totalDots);
					const labelRadius = dotSpacing + 10;
					const cx = 150;
					const cy = 150;
					const angle = -((i / totalDots) * 2 * Math.PI) - Math.PI / 2; // counter-clockwise
					const x = cx + Math.cos(angle) * labelRadius;
					const y = cy + Math.sin(angle) * labelRadius;

					// Calculate text anchor based on position to avoid overlap with dots
					let anchor = 'middle';
					let baseline = 'middle';
					const normalizedAngle = (((angle + Math.PI / 2) % (2 * Math.PI)) + 2 * Math.PI) % (2 * Math.PI);

					// Right side (3 o'clock area)
					if (normalizedAngle > Math.PI * 0.25 && normalizedAngle < Math.PI * 0.75) {
						anchor = 'start';
					}
					// Left side (9 o'clock area)
					else if (normalizedAngle > Math.PI * 1.25 && normalizedAngle < Math.PI * 1.75) {
						anchor = 'end';
					}
					// Top area (12 o'clock)
					else if (normalizedAngle < Math.PI * 0.25 || normalizedAngle > Math.PI * 1.75) {
						baseline = 'auto';
					}
					// Bottom area (6 o'clock)
					else if (normalizedAngle > Math.PI * 0.75 && normalizedAngle < Math.PI * 1.25) {
						baseline = 'hanging';
					}

					return (
						<text key={`${label}-${index}`} x={x} y={y} className="clock-num" textAnchor={anchor as any} dominantBaseline={baseline as any}>
							{formatPrice(label)}
						</text>
					);
				})}

				{/* Center group for foreignObject */}
				<g className="clock-center" transform="translate(150,150) scale(0.85)">
					<foreignObject x="-100" y="-100" width="200" height="200">
						<div className="clock-center-panel">
							{/* Top row */}
							<div className="clock-row">
								<div className="clock-field">
									<span className="clock-label">
										<i className="bi bi-arrow-repeat me-1"></i>
										{t('rounds')}
									</span>
									<div className="clock-box">
										<span className="clock-value">{round}</span>
									</div>
								</div>

								<div className="clock-field">
									<span className="clock-label">
										<i className="bi bi-coin me-1"></i>
										{t('munt')}
									</span>
									<div className="clock-box">
										<span className="clock-value">{coin}</span>
									</div>
								</div>
							</div>

							{/* Second row */}
							<div className="clock-row">
								<div className="clock-field">
									<span className="clock-label">
										<i className="bi bi-inbox-fill me-1"></i>
										{t('aant_stock')}
									</span>
									<div className="clock-box">
										<span className="clock-value">{amountStock}</span>
									</div>
								</div>
							</div>

							{/* Third row */}
							<div className="clock-row clock-row--wide">
								<div className="clock-field clock-field--price">
									<span className="clock-label">
										<i className="bi bi-currency-euro me-1"></i>
										{t('price')}
									</span>
									<div className="clock-box clock-box--large">
										<span className="clock-value clock-value--large">{formatEur(currentPrice)}</span>
									</div>
								</div>

								<div className="clock-field">
									<span className="clock-label">{t('min_aant')}</span>
									<div className="clock-box">
										<span className="clock-value">{minAmount}</span>
									</div>
								</div>
							</div>
						</div>
					</foreignObject>
				</g>
			</svg>
		</div>
	);
});

AuctionClock.displayName = 'AuctionClock';
export default AuctionClock;
