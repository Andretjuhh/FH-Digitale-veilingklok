import { fetchResponse } from '../utils/fetchHelpers';
import { NewKwekerAccount } from '../declarations/KwekerAccount';

export async function createKwekerAccount(account: NewKwekerAccount) {
	return await fetchResponse('/kweker/create', {
		method: 'POST',
		body: JSON.stringify(account),
	});
}
