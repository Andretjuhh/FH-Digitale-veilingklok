import { fetchResponse } from '../utils/fetchHelpers';
import { NewVeilingmeesterAccount } from '../declarations/VeilingmeesterAccount';
import { AuthResponse } from '../declarations/AuthenticationResponse';
import { VeilingklokkenDetails } from '../declarations/VeilingklokkenDetails';

export async function createVeilingmeesterAccount(account: NewVeilingmeesterAccount) {
    return await fetchResponse<AuthResponse>('/api/veilingmeester/create', {
        method: 'POST',
        body: JSON.stringify(account),
    });
}

export async function getVeilingmeesterAccountInfo() {
    return await fetchResponse('/api/veiilingmeester/account-info', {
        method: 'GET',
    });
}

export async function getVeilingmeesterVeilingklokken() {
    return await fetchResponse<{ veilingklokken: VeilingklokkenDetails[] }>('/api/veilingmeester/veilingklokken', {
        method: 'GET',
    });
}