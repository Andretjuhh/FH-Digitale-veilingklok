// External imports
import {useReducer} from 'react';

// Type
type State = {
	type: 'succeed' | 'error' | 'loading' | 'idle';
	message: string | string[];
};
export type Action = {
	type: 'succeed' | 'error' | 'loading' | 'idle';
	message?: string | string[];
}

const stateReducer = (state: State, action: Action): State => {
	switch (action.type) {
		case "idle":
			return {
				...state,
				type: 'idle',
				message: '',
			};
		case "succeed":
			return {
				...state,
				type: 'succeed',
				message: action.message || '',
			};
		case "error":
			return {
				...state,
				type: 'error',
				message: action.message || '',
			};
		case "loading":
			return {
				...state,
				type: 'loading',
				message: action.message || '',
			};
		default:
			throw new Error(`Unhandled component state type: ${action.type}`);
	}
};

const useComponentStateReducer = (_default?: State) => {
	return useReducer(stateReducer, _default || {type: 'idle', message: ''});
}

export {useComponentStateReducer};