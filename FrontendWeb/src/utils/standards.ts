import {OrderStatus} from "../declarations/enums/OrderStatus";
import {VeilingKlokStatus} from "../declarations/enums/VeilingKlokStatus";

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

export const getRandomColor = (name: string) => {
	const colors = ['#EF4444', '#F59E0B', '#10B981', '#3B82F6', '#6366F1', '#8B5CF6', '#EC4899'];
	let hash = 0;
	for (let i = 0; i < name.length; i++) {
		hash = name.charCodeAt(i) + ((hash << 5) - hash);
	}
	return colors[Math.abs(hash) % colors.length];
};

export const sanitizeMessage = (value: string): string => value.replace(/\\"/g, '"').replace(/"/g, '').replace(/\s+/g, ' ').trim();

export function formatEur(value: number) {
	return value.toLocaleString('nl-NL', {style: 'currency', currency: 'EUR', minimumFractionDigits: 2});
}

export function getOrderStatusString(status: number | string): string {
	let normalizedStatus: OrderStatus;
	let statusKey: string;

	if (typeof status === 'string' && isNaN(Number(status))) {
		// If status is passed as string key e.g. "Open"
		statusKey = status;
		normalizedStatus = (OrderStatus as any)[status];
	} else {
		// If status is passed as number or string number e.g. 0 or "0"
		normalizedStatus = typeof status === 'string' ? parseInt(status) : status;
		statusKey = OrderStatus[normalizedStatus];
	}
	return statusKey.toString();
}

export function getNormalizedOrderStatus(status: number | string): OrderStatus | null {
	if (typeof status === 'string' && isNaN(Number(status))) {
		// If status is passed as string key e.g. "Open"
		return (OrderStatus as any)[status] ?? null;
	} else {
		// If status is passed as number or string number e.g. 0 or "0"
		return typeof status === 'string' ? parseInt(status) : status;
	}
}

export function getVeilingKlokStatusString(status: number | string): string {
	let normalizedStatus: VeilingKlokStatus;
	let statusKey: string;

	if (typeof status === 'string' && isNaN(Number(status))) {
		statusKey = status;
		normalizedStatus = (VeilingKlokStatus as any)[status];
	} else {
		normalizedStatus = typeof status === 'string' ? parseInt(status) : status;
		statusKey = VeilingKlokStatus[normalizedStatus];
	}
	return statusKey?.toString() || '';
}

export function getNormalizedVeilingKlokStatus(status: number | string): VeilingKlokStatus | null {
	if (typeof status === 'string' && isNaN(Number(status))) {
		return (VeilingKlokStatus as any)[status] ?? null;
	} else {
		return typeof status === 'string' ? parseInt(status) : status;
	}
}

export function toIsoStringWithOffset(date: Date) {
	const tzo = -date.getTimezoneOffset(),
		dif = tzo >= 0 ? '+' : '-',
		pad = (num: number) => {
			const norm = Math.floor(Math.abs(num));
			return (norm < 10 ? '0' : '') + norm;
		};
	return date.getFullYear() +
		'-' + pad(date.getMonth() + 1) +
		'-' + pad(date.getDate()) +
		'T' + pad(date.getHours()) +
		':' + pad(date.getMinutes()) +
		':' + pad(date.getSeconds()) +
		dif + pad(tzo / 60) + ':' + pad(tzo % 60);
}

export function formatDate(dateString: string, locale: string = 'en-US', type: number = 0): string {
	if (type == 0) {
		return new Date(dateString)
			.toLocaleString('en-US', {
				day: 'numeric',
				month: 'short',
				year: 'numeric',
				hour: 'numeric',
				minute: '2-digit',
				hour12: true
			})
			.toUpperCase()      // Changes "jan" to "JAN" and "am" to "AM"
			.replace(/\./g, '') // Removes dots from "A.M."
			.replace(',', '');
	} else if (type == 1) {
		return new Date(dateString)
			.toLocaleDateString(locale, {
				day: '2-digit',
				month: '2-digit',
				year: 'numeric',
			});
	} else if (type == 2) {
		return new Date(dateString)
			.toLocaleString(locale, {
				day: '2-digit',
				month: '2-digit',
				year: 'numeric',
				hour: '2-digit',
				minute: '2-digit',
			});
	} else if (type == 3) {
		return new Date(dateString).toLocaleString(locale, {
			weekday: 'long',
			year: 'numeric',
			month: 'long',
			day: 'numeric',
			hour: '2-digit',
			minute: '2-digit'
		});
	}

	return '-- -- ----';
}
