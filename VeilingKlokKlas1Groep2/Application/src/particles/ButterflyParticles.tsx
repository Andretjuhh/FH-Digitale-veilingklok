import React, {useEffect, useRef, useState, useCallback} from 'react';

// Define the interface for the component's props, even if empty for now.
// This allows for future customization (e.g., passing in custom colors or count).
interface ButterflyParticlesProps {
}

// === Configuration ===
const PRIMARY_COLOR: string = '#0cc988'; // primary-500
const SECONDARY_COLOR: string = '#361280'; // secondary-500
const MAX_PARTICLES: number = 60;

// === Butterfly Class Definition ===
class Butterfly {
	x: number;
	y: number;
	vx: number;
	vy: number;
	color: string;
	baseSize: number;
	flutterSpeed: number;
	angle: number;
	swirlSpeed: number;
	age: number;

	constructor(canvasWidth: number, canvasHeight: number) {
		this.x = 0;
		this.y = 0;
		this.vx = 0;
		this.vy = 0;
		this.angle = 0;
		this.swirlSpeed = 0;
		this.age = 0;

		this.init(canvasWidth, canvasHeight);
		this.color = Math.random() < 0.5 ? PRIMARY_COLOR : SECONDARY_COLOR;
		this.baseSize = Math.random() * 2 + 1.5; // Size between 1.5 and 3.5
		this.flutterSpeed = Math.random() * 0.08 + 0.04; // Reduced initial flutter speed
	}

	// Initialization/Recycling function
	init(canvasWidth: number, canvasHeight: number): void {
		// Spawn Location: Primarily on the right side (50% to 100% of the width)
		this.x = canvasWidth * (0.5 + Math.random() * 0.5);
		// Spawn randomly across the vertical height
		this.y = Math.random() * canvasHeight;

		// Swirling Movement: Reduced velocity for slower, gentle movement
		this.vx = (Math.random() - 0.5) * 0.4; // Horizontal drift (-0.2 to 0.2)
		this.vy = (Math.random() - 0.5) * 0.4; // Vertical drift (-0.2 to 0.2)

		// Additional parameters for dynamic movement
		this.angle = Math.random() * Math.PI * 2;
		this.swirlSpeed = Math.random() * 0.01 + 0.005; // Slightly reduced swirl speed
		this.age = 0; // Reset age on initialization
	}

	update(canvasWidth: number, canvasHeight: number): void {
		this.age++;

		// Update the main swirl angle based on age/time
		this.angle += this.swirlSpeed;

		// Apply movement: Reduced swirl amplitude for gentler motion
		this.x += this.vx + Math.sin(this.angle) * 0.25;
		this.y += this.vy + Math.cos(this.angle) * 0.25;

		// The wings still flutter based on flutterSpeed for the animation
		this.flutterSpeed += 0.025; // Significantly reduced rate for slower flapping visual

		// Recycling Logic: If the butterfly goes off-screen, re-initialize it
		const padding: number = 100;
		if (this.y < -padding || this.y > canvasHeight + padding || this.x < -padding || this.x > canvasWidth + padding) {
			this.init(canvasWidth, canvasHeight);
			this.color = Math.random() < 0.5 ? PRIMARY_COLOR : SECONDARY_COLOR;
		}
	}

	draw(ctx: CanvasRenderingContext2D): void {
		const size: number = this.baseSize;
		// Flap amplitude remains high but is applied at a slower rate
		const flap: number = Math.sin(this.flutterSpeed * 2) * 0.9;

		ctx.fillStyle = this.color;
		ctx.globalAlpha = Math.min(0.8, this.age / 80); // Set max alpha to 0.8 for a delicate look

		// Save current canvas state (important for transformations)
		ctx.save();
		ctx.translate(this.x, this.y); // Move origin to butterfly's center
		// Rotate based on movement and flap for dynamic tilt
		ctx.rotate(this.vx * 7 + flap * 0.2);

		// Body (simple small arc for the core)
		ctx.beginPath();
		ctx.arc(0, 0, size * 0.3, 0, Math.PI * 2);
		ctx.fill();

		const wingLength: number = size * (2.5 - flap * 0.5); // Wings get shorter as they flap inward
		const wingWidth: number = size * 1.5;

		// --- Left Wing (using Bezier curves for organic shape) ---
		ctx.beginPath();
		ctx.moveTo(0, 0);

		// Top curve (Upper wing)
		ctx.bezierCurveTo(
			-wingWidth, -size * 0.5,
			-wingLength * 0.5, -wingLength * 0.8,
			-size * 0.2, -wingLength
		);

		// Bottom curve (Lower wing)
		ctx.bezierCurveTo(
			-wingLength * 0.8, wingLength * 0.2,
			-wingWidth * 0.2, wingWidth,
			0, 0
		);
		ctx.fill();

		// --- Right Wing (using Bezier curves for organic shape) ---
		ctx.beginPath();
		ctx.moveTo(0, 0);

		// Top curve (Upper wing)
		ctx.bezierCurveTo(
			wingWidth, -size * 0.5,
			wingLength * 0.5, -wingLength * 0.8,
			size * 0.2, -wingLength
		);

		// Bottom curve (Lower wing)
		ctx.bezierCurveTo(
			wingLength * 0.8, wingLength * 0.2,
			wingWidth * 0.2, wingWidth,
			0, 0
		);
		ctx.fill();

		ctx.restore(); // Restore canvas state
		ctx.globalAlpha = 1; // Reset alpha
	}
}

// === Reusable React Component (Functional Component with TypeScript) ===
const ButterflyParticles: React.FC<ButterflyParticlesProps> = () => {
	const canvasRef = useRef<HTMLCanvasElement>(null);
	const [particles, setParticles] = useState<Butterfly[]>([]);
	const [isReady, setIsReady] = useState<boolean>(false);

	// Initialize particles once
	useEffect(() => {
		const initialParticles: Butterfly[] = [];
		const canvas = canvasRef.current;
		if (canvas) {
			for (let i = 0; i < MAX_PARTICLES; i++) {
				initialParticles.push(new Butterfly(window.innerWidth, window.innerHeight));
			}
			setParticles(initialParticles);
			setIsReady(true);
		}
	}, []);

	// Animation Loop
	const animate = useCallback(() => {
		const canvas = canvasRef.current;
		if (!canvas || !isReady) return;

		// Type the context explicitly
		const ctx = canvas.getContext('2d') as CanvasRenderingContext2D;
		const dpr: number = window.devicePixelRatio || 1;

		const width: number = window.innerWidth;
		const height: number = window.innerHeight;

		// Handle high-DPI scaling and resizing
		if (canvas.width !== width * dpr || canvas.height !== height * dpr) {
			canvas.width = width * dpr;
			canvas.height = height * dpr;
			canvas.style.width = `${width}px`;
			canvas.style.height = `${height}px`;
			ctx.scale(dpr, dpr);
		}

		// Clear the canvas for the next frame
		ctx.clearRect(0, 0, width, height);

		particles.forEach((p: Butterfly) => {
			p.update(width, height);
			p.draw(ctx);
		});

		requestAnimationFrame(animate);
	}, [particles, isReady]);

	// Start animation loop and handle resize
	useEffect(() => {
		if (isReady) {
			const animationFrame: number = requestAnimationFrame(animate);

			let resizeTimer: number | NodeJS.Timeout;
			const handleResize = () => {
				clearTimeout(resizeTimer);
				// Throttle resize updates for performance
				resizeTimer = setTimeout(() => {
					const newParticles: Butterfly[] = particles.map((p: Butterfly) => {
						// Re-init to redistribute particles across the new screen size
						p.init(window.innerWidth, window.innerHeight);
						return p;
					});
					setParticles(newParticles);
				}, 300);
			};

			window.addEventListener('resize', handleResize);

			return () => {
				cancelAnimationFrame(animationFrame);
				window.removeEventListener('resize', handleResize);
			};
		}
	}, [isReady, animate, particles]); // Added 'particles' to dep array for resize handler

	return (
		// The component is designed to be an absolute overlay covering the entire screen.
		// The 'pointer-events-none' class ensures user interactions aren't blocked by the canvas.
		<canvas
			ref={canvasRef}
			className="absolute inset-0 w-screen h-screen z-10 pointer-events-none z-0"
		/>
	);
};

export default ButterflyParticles;