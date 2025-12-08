export async function delay(ms: number): Promise<void> {
	return new Promise((resolve) => setTimeout(resolve, ms));
}

export const isDevelopment = () => true;

export const sanitizeMessage = (value: string): string => value.replace(/\\"/g, '"').replace(/"/g, '').replace(/\s+/g, ' ').trim();
