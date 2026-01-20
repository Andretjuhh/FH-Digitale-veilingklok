// External imports
import Logger from '../../utils/logger';

namespace LocalStorageService {
	export function getItem<T = any>(key: string): T | null {
		try {
			const item = localStorage.getItem(key);
			if (item) {
				try {
					return JSON.parse(item);
				} catch {
					return item as T;
				}
			}
			return null;
		} catch (error) {
			Logger.error('Error reading from storage:', error);
			return null;
		}
	}

	export function setItem(key: string, value: any) {
		try {
			if (value === null) {
				localStorage.removeItem(key);
			} else {
				const stringValue = JSON.stringify(value);
				localStorage.setItem(key, stringValue);
			}
		} catch (error) {
			Logger.error('Error writing to storage:', error);
		}
	}

	export function removeItem(key: string) {
		try {
			localStorage.removeItem(key);
		} catch (error) {
			Logger.error('Error removing from storage:', error);
		}
	}
}

export { LocalStorageService };
