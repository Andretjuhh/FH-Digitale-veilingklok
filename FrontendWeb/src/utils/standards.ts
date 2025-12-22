export async function delay(ms: number): Promise<void> {
	return new Promise((resolve) => setTimeout(resolve, ms));
}

export const isDevelopment = () => true;

export function getRandomColorHSL(): string {
	const hue = Math.floor(Math.random() * 360);
	const saturation = 70 + Math.random() * 30; // 70–100%
	const lightness = 40 + Math.random() * 20;  // 40–60%

	return `hsl(${hue}, ${saturation}%, ${lightness}%)`;
}

export const sanitizeMessage = (value: string): string => value.replace(/\\"/g, '"').replace(/"/g, '').replace(/\s+/g, ' ').trim();
