/**
 * Defines the type of storage to use.
 * 'persistent' maps to localStorage.
 * 'temporary' maps to sessionStorage.
 */
export type StorageType = 'persistent' | 'temporary';

/**
 * Stores a value in the specified storage type.
 */
export const setStorage = (key: string, value: any, type: StorageType) => {
	const storage = type === 'persistent' ? localStorage : sessionStorage;
	storage.setItem(key, JSON.stringify(value));
};

/**
 * Retrieves a value from storage.
 * It checks sessionStorage first, then localStorage.
 */
export const getStorage = (key: string) => {
	const fromSession = sessionStorage.getItem(key);
	if (fromSession) {
		try {
			return JSON.parse(fromSession);
		} catch (e) {
			return fromSession;
		}
	}

	const fromLocal = localStorage.getItem(key);
	if (fromLocal) {
		try {
			return JSON.parse(fromLocal);
		} catch (e) {
			return fromLocal;
		}
	}

	return null;
};

/**
 * Removes a value from both sessionStorage and localStorage.
 */
export const removeStorage = (key: string) => {
	sessionStorage.removeItem(key);
	localStorage.removeItem(key);
};

export default {
	setItem: setStorage,
	getItem: getStorage,
	removeItem: removeStorage,
};



