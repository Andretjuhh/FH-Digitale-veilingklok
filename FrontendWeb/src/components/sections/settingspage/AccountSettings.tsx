// External imports
import React, { useEffect, useState } from 'react';

// Internal imports
import { useRootContext } from '../../contexts/RootContext';
import { AccountType } from '../../../declarations/enums/AccountTypes';
import { updateKoperAccount } from '../../../controllers/server/koper';
import { updateKwekerAccount } from '../../../controllers/server/kweker';
import { updateVeilingmeesterAccount } from '../../../controllers/server/veilingmeester';
import { isHttpError } from '../../../declarations/types/HttpError';

type NoticeType = 'idle' | 'saving' | 'success' | 'error';

type AddressState = {
	street: string;
	city: string;
	regionOrState: string;
	postalCode: string;
	country: string;
};

type FormState = {
	firstName: string;
	lastName: string;
	companyName: string;
	kvkNumber: string;
	email: string;
	telephone: string;
	region: string;
	password: string;
	address: AddressState;
};

const emptyAddress: AddressState = {
	street: '',
	city: '',
	regionOrState: '',
	postalCode: '',
	country: '',
};

function AccountSettings() {
	const { account, refreshAccount } = useRootContext();
	const accountTypeValue = account?.accountType;
	const accountType =
		typeof accountTypeValue === 'string'
			? accountTypeValue
			: accountTypeValue === 0
				? AccountType.Koper
				: accountTypeValue === 1
					? AccountType.Kweker
					: accountTypeValue === 2
						? AccountType.Veilingmeester
						: undefined;
	const isKweker = accountType === AccountType.Kweker;
	const isVeilingmeester = accountType === AccountType.Veilingmeester;
	const isKoper = accountType === AccountType.Koper;

	const [notice, setNotice] = useState<{ type: NoticeType; message: string }>({
		type: 'idle',
		message: '',
	});

	const [form, setForm] = useState<FormState>({
		firstName: '',
		lastName: '',
		companyName: '',
		kvkNumber: '',
		email: '',
		telephone: '',
		region: '',
		password: '',
		address: emptyAddress,
	});
	const [showPassword, setShowPassword] = useState(false);
	const [allowPasswordChange, setAllowPasswordChange] = useState(false);
	const [editableFields, setEditableFields] = useState<Record<string, boolean>>({});
	const [isAddressExpanded, setIsAddressExpanded] = useState(false);

	useEffect(() => {
		if (!account) return;
		const primaryAddress =
			account.address ??
			account.addresses?.find((addr) => addr.id === account.primaryAddressId) ??
			account.addresses?.[0];

		setForm({
			firstName: account.firstName ?? '',
			lastName: account.lastName ?? '',
			companyName: account.companyName ?? '',
			kvkNumber: account.kvkNumber ?? '',
			email: account.email ?? '',
			telephone: account.telephone ?? '',
			region: account.region ?? '',
			password: '',
			address: primaryAddress
				? {
					street: primaryAddress.street ?? '',
					city: primaryAddress.city ?? '',
					regionOrState: primaryAddress.regionOrState ?? '',
					postalCode: primaryAddress.postalCode ?? '',
					country: primaryAddress.country ?? '',
				}
				: emptyAddress,
		});
		setAllowPasswordChange(accountType === AccountType.Koper);
		setShowPassword(false);
		setEditableFields({});
		setIsAddressExpanded(false);
	}, [account]);

	const isEditable = (key: string) => !!editableFields[key];
	const toggleEditable = (key: string) => {
		setEditableFields((prev) => ({ ...prev, [key]: !prev[key] }));
	};

	const updateField = (field: keyof FormState) => (event: React.ChangeEvent<HTMLInputElement>) => {
		setForm((prev) => ({ ...prev, [field]: event.target.value }));
	};

	const updateAddressField = (field: keyof AddressState) => (event: React.ChangeEvent<HTMLInputElement>) => {
		setForm((prev) => ({ ...prev, address: { ...prev.address, [field]: event.target.value } }));
	};

	const getTrimmed = (value: string) => value.trim();
	const getOptional = (value: string) => {
		const trimmed = value.trim();
		return trimmed.length ? trimmed : undefined;
	};

	const buildAddress = () => ({
		street: getTrimmed(form.address.street),
		city: getTrimmed(form.address.city),
		regionOrState: getTrimmed(form.address.regionOrState),
		postalCode: getTrimmed(form.address.postalCode),
		country: getTrimmed(form.address.country),
	});

	const hasAnyAddressValue = () => {
		const values = Object.values(buildAddress());
		return values.some((value) => value.length > 0);
	};

	const handleSave = async () => {
		if (!accountType) {
			setNotice({ type: 'error', message: 'Account information is not available yet.' });
			return;
		}

		setNotice({ type: 'saving', message: 'Saving changes...' });

		try {
			if (isKoper) {
				const payload = {
					email: getTrimmed(form.email),
					password: getTrimmed(form.password),
					firstName: getTrimmed(form.firstName || account?.firstName || ''),
					lastName: getTrimmed(form.lastName || account?.lastName || ''),
					telephone: getTrimmed(form.telephone),
					address: buildAddress(),
				};

				if (!payload.email || !payload.password || !payload.firstName || !payload.lastName || !payload.telephone) {
					setNotice({ type: 'error', message: 'Please fill in all required fields.' });
					return;
				}

				if (Object.values(payload.address).some((value) => value.length === 0)) {
					setNotice({ type: 'error', message: 'Please complete the full address.' });
					return;
				}

				await updateKoperAccount(payload);
			} else if (isKweker) {
				const payload = {
					companyName: getOptional(form.companyName),
					email: getOptional(form.email),
					password: allowPasswordChange ? getOptional(form.password) : undefined,
					telephone: getOptional(form.telephone),
					address: hasAnyAddressValue() ? buildAddress() : undefined,
				};

				const hasChanges = Object.values(payload).some((value) => value !== undefined);
				if (!hasChanges) {
					setNotice({ type: 'error', message: 'No changes to save.' });
					return;
				}

				await updateKwekerAccount(payload);
			} else if (isVeilingmeester) {
				const payload = {
					email: getOptional(form.email),
					password: getOptional(form.password),
					regio: getOptional(form.region),
				};

				const hasChanges = Object.values(payload).some((value) => value !== undefined);
				if (!hasChanges) {
					setNotice({ type: 'error', message: 'No changes to save.' });
					return;
				}

				await updateVeilingmeesterAccount(payload);
			}

			await refreshAccount();
			setForm((prev) => ({ ...prev, password: '' }));
			setNotice({ type: 'success', message: 'Account updated successfully.' });
		} catch (error) {
			if (isHttpError(error)) setNotice({ type: 'error', message: error.message });
			else setNotice({ type: 'error', message: 'Something went wrong while saving.' });
		}
	};

	return (
		<section className="settings-panel">
			<header className="settings-panel-header">
				<h2>Account</h2>
				<p>Update your profile details and password.</p>
			</header>
			<div className="settings-panel-body">
				{notice.type !== 'idle' && (
					<div className={`settings-notice ${notice.type}`}>{notice.message}</div>
				)}
				{isKweker && (
					<div className="settings-field">
						<label htmlFor="companyName">Company name</label>
						<div className="settings-field-row">
							<input
								id="companyName"
								className="settings-input"
								type="text"
								placeholder="Company name"
								value={form.companyName}
								onChange={updateField('companyName')}
								readOnly={!isEditable('companyName')}
							/>
							<button
								className="settings-edit-btn"
								type="button"
								onClick={() => toggleEditable('companyName')}
								aria-label="Edit company name"
							>
								<i className="bi bi-pencil" />
							</button>
						</div>
					</div>
				)}
				{!accountType && (
					<>
						<div className="settings-field">
							<label htmlFor="firstName">First name</label>
							<input
								id="firstName"
								className="settings-input"
								type="text"
								placeholder="First name"
								value={form.firstName}
								onChange={updateField('firstName')}
							/>
						</div>
						<div className="settings-field">
							<label htmlFor="lastName">Last name</label>
							<input
								id="lastName"
								className="settings-input"
								type="text"
								placeholder="Last name"
								value={form.lastName}
								onChange={updateField('lastName')}
							/>
						</div>
					</>
				)}
				<div className="settings-field">
					<label htmlFor="email">Email address</label>
					<div className="settings-field-row">
						<input
							id="email"
							className="settings-input"
							type="email"
							placeholder="name@example.com"
							value={form.email}
							onChange={updateField('email')}
							readOnly={!isEditable('email')}
						/>
						<button
							className="settings-edit-btn"
							type="button"
							onClick={() => toggleEditable('email')}
							aria-label="Edit email"
						>
							<i className="bi bi-pencil" />
						</button>
					</div>
				</div>
				{(isKoper || isKweker) && (
					<div className="settings-field">
						<label htmlFor="telephone">Phone number</label>
						<div className="settings-field-row">
							<input
								id="telephone"
								className="settings-input"
								type="tel"
								placeholder="+31 6 12345678"
								value={form.telephone}
								onChange={updateField('telephone')}
								readOnly={!isEditable('telephone')}
							/>
							<button
								className="settings-edit-btn"
								type="button"
								onClick={() => toggleEditable('telephone')}
								aria-label="Edit phone number"
							>
								<i className="bi bi-pencil" />
							</button>
						</div>
					</div>
				)}
				{isVeilingmeester && (
					<div className="settings-field">
						<label htmlFor="region">Region</label>
						<div className="settings-field-row">
							<input
								id="region"
								className="settings-input"
								type="text"
								placeholder="Noord-Holland"
								value={form.region}
								onChange={updateField('region')}
								readOnly={!isEditable('region')}
							/>
							<button
								className="settings-edit-btn"
								type="button"
								onClick={() => toggleEditable('region')}
								aria-label="Edit region"
							>
								<i className="bi bi-pencil" />
							</button>
						</div>
					</div>
				)}
				{(isKoper || isKweker) && (
					<>
						<div className="settings-section-toggle">
							<button
								className="settings-link-btn"
								type="button"
								onClick={() => setIsAddressExpanded((prev) => !prev)}
							>
								{isAddressExpanded ? 'Verberg adres' : 'Adres wijzigen'}
							</button>
						</div>
						{isAddressExpanded && (
							<>
						<div className="settings-field">
							<label htmlFor="street">{isKoper ? 'Adres' : 'Street'}</label>
							<div className="settings-field-row">
								<input
									id="street"
									className="settings-input"
									type="text"
									placeholder={isKoper ? 'Adres' : 'Street'}
									value={form.address.street}
									onChange={updateAddressField('street')}
									readOnly={!isEditable('address.street')}
								/>
								<button
									className="settings-edit-btn"
									type="button"
									onClick={() => toggleEditable('address.street')}
									aria-label="Edit address"
								>
									<i className="bi bi-pencil" />
								</button>
							</div>
						</div>
						<div className="settings-field">
							<label htmlFor="city">{isKoper ? 'Plaats' : 'City'}</label>
							<div className="settings-field-row">
								<input
									id="city"
									className="settings-input"
									type="text"
									placeholder={isKoper ? 'Plaats' : 'City'}
									value={form.address.city}
									onChange={updateAddressField('city')}
									readOnly={!isEditable('address.city')}
								/>
								<button
									className="settings-edit-btn"
									type="button"
									onClick={() => toggleEditable('address.city')}
									aria-label="Edit city"
								>
									<i className="bi bi-pencil" />
								</button>
							</div>
						</div>
						<div className="settings-field">
							<label htmlFor="regionOrState">{isKoper ? 'Regio' : 'Region or state'}</label>
							<div className="settings-field-row">
								<input
									id="regionOrState"
									className="settings-input"
									type="text"
									placeholder={isKoper ? 'Regio' : 'Region or state'}
									value={form.address.regionOrState}
									onChange={updateAddressField('regionOrState')}
									readOnly={!isEditable('address.regionOrState')}
								/>
								<button
									className="settings-edit-btn"
									type="button"
									onClick={() => toggleEditable('address.regionOrState')}
									aria-label="Edit region"
								>
									<i className="bi bi-pencil" />
								</button>
							</div>
						</div>
						<div className="settings-field">
							<label htmlFor="postalCode">{isKoper ? 'Postcode' : 'Postal code'}</label>
							<div className="settings-field-row">
								<input
									id="postalCode"
									className="settings-input"
									type="text"
									placeholder={isKoper ? 'Postcode' : 'Postal code'}
									value={form.address.postalCode}
									onChange={updateAddressField('postalCode')}
									readOnly={!isEditable('address.postalCode')}
								/>
								<button
									className="settings-edit-btn"
									type="button"
									onClick={() => toggleEditable('address.postalCode')}
									aria-label="Edit postal code"
								>
									<i className="bi bi-pencil" />
								</button>
							</div>
						</div>
						<div className="settings-field">
							<label htmlFor="country">{isKoper ? 'Land' : 'Country'}</label>
							<div className="settings-field-row">
								<input
									id="country"
									className="settings-input"
									type="text"
									placeholder={isKoper ? 'Land' : 'Country'}
									value={form.address.country}
									onChange={updateAddressField('country')}
									readOnly={!isEditable('address.country')}
								/>
								<button
									className="settings-edit-btn"
									type="button"
									onClick={() => toggleEditable('address.country')}
									aria-label="Edit country"
								>
									<i className="bi bi-pencil" />
								</button>
							</div>
						</div>
							</>
						)}
					</>
				)}
				<div className="settings-field">
					<label htmlFor="password">
						{isKoper ? 'Password (required)' : 'New password'}
					</label>
					<div className="settings-password-row">
						<input
							id="password"
							className="settings-input"
							type={showPassword ? 'text' : 'password'}
							placeholder={allowPasswordChange || isKoper ? '********' : 'Click change password'}
							value={form.password}
							onChange={updateField('password')}
							autoComplete="new-password"
							disabled={!allowPasswordChange && !isKoper}
						/>
						<button
							className="settings-link-btn"
							type="button"
							onClick={() => setShowPassword((prev) => !prev)}
						>
							{showPassword ? 'Hide' : 'Show'}
						</button>
					</div>
					<div className="settings-inline-actions">
						<button
							className="settings-link-btn"
							type="button"
							onClick={() => setAllowPasswordChange((prev) => !prev)}
						>
							{allowPasswordChange ? 'Cancel password change' : 'Change password'}
						</button>
					</div>
				</div>
				<button className="settings-primary-btn" type="button" onClick={handleSave} disabled={notice.type === 'saving'}>
					{notice.type === 'saving' ? 'Saving...' : 'Save changes'}
				</button>
			</div>
		</section>
	);
}

export default AccountSettings;
