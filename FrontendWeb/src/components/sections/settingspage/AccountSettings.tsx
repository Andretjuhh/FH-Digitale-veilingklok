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
	const { account, refreshAccount, t } = useRootContext();
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
			setNotice({ type: 'error', message: t('account_notice_not_ready') });
			return;
		}

		setNotice({ type: 'saving', message: t('account_notice_saving') });

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
					setNotice({ type: 'error', message: t('account_notice_required') });
					return;
				}

				if (Object.values(payload.address).some((value) => value.length === 0)) {
					setNotice({ type: 'error', message: t('account_notice_complete_address') });
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
					setNotice({ type: 'error', message: t('account_notice_no_changes') });
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
					setNotice({ type: 'error', message: t('account_notice_no_changes') });
					return;
				}

				await updateVeilingmeesterAccount(payload);
			}

			await refreshAccount();
			setForm((prev) => ({ ...prev, password: '' }));
			setNotice({ type: 'success', message: t('account_notice_updated') });
		} catch (error) {
			if (isHttpError(error)) setNotice({ type: 'error', message: error.message });
			else setNotice({ type: 'error', message: t('account_notice_error') });
		}
	};

	return (
		<section className="settings-panel">
			<header className="settings-panel-header">
				<h2>{t('account_settings_title')}</h2>
				<p>{t('account_settings_subtitle')}</p>
			</header>
			<div className="settings-panel-body">
				{notice.type !== 'idle' && (
					<div className={`settings-notice ${notice.type}`}>{notice.message}</div>
				)}
				{isKweker && (
					<div className="settings-field">
						<label htmlFor="companyName">{t('account_company_name_label')}</label>
						<div className="settings-field-row">
							<input
								id="companyName"
								className="settings-input"
								type="text"
								placeholder={t('account_company_name_placeholder')}
								value={form.companyName}
								onChange={updateField('companyName')}
								readOnly={!isEditable('companyName')}
							/>
							<button
								className="settings-edit-btn"
								type="button"
								onClick={() => toggleEditable('companyName')}
								aria-label={t('account_edit_company_name_aria')}
							>
								<i className="bi bi-pencil" />
							</button>
						</div>
					</div>
				)}
				{!accountType && (
					<>
						<div className="settings-field">
							<label htmlFor="firstName">{t('account_first_name_label')}</label>
							<input
								id="firstName"
								className="settings-input"
								type="text"
								placeholder={t('account_first_name_placeholder')}
								value={form.firstName}
								onChange={updateField('firstName')}
							/>
						</div>
						<div className="settings-field">
							<label htmlFor="lastName">{t('account_last_name_label')}</label>
							<input
								id="lastName"
								className="settings-input"
								type="text"
								placeholder={t('account_last_name_placeholder')}
								value={form.lastName}
								onChange={updateField('lastName')}
							/>
						</div>
					</>
				)}
				<div className="settings-field">
					<label htmlFor="email">{t('account_email_label')}</label>
					<div className="settings-field-row">
						<input
							id="email"
							className="settings-input"
							type="email"
							placeholder={t('account_email_placeholder')}
							value={form.email}
							onChange={updateField('email')}
							readOnly={!isEditable('email')}
						/>
						<button
							className="settings-edit-btn"
							type="button"
							onClick={() => toggleEditable('email')}
							aria-label={t('account_edit_email_aria')}
						>
							<i className="bi bi-pencil" />
						</button>
					</div>
				</div>
				{(isKoper || isKweker) && (
					<div className="settings-field">
						<label htmlFor="telephone">{t('account_phone_label')}</label>
						<div className="settings-field-row">
							<input
								id="telephone"
								className="settings-input"
								type="tel"
								placeholder={t('account_phone_placeholder')}
								value={form.telephone}
								onChange={updateField('telephone')}
								readOnly={!isEditable('telephone')}
							/>
							<button
								className="settings-edit-btn"
								type="button"
								onClick={() => toggleEditable('telephone')}
								aria-label={t('account_edit_phone_aria')}
							>
								<i className="bi bi-pencil" />
							</button>
						</div>
					</div>
				)}
				{isVeilingmeester && (
					<div className="settings-field">
						<label htmlFor="region">{t('account_region_label')}</label>
						<div className="settings-field-row">
							<input
								id="region"
								className="settings-input"
								type="text"
								placeholder={t('account_region_placeholder')}
								value={form.region}
								onChange={updateField('region')}
								readOnly={!isEditable('region')}
							/>
							<button
								className="settings-edit-btn"
								type="button"
								onClick={() => toggleEditable('region')}
								aria-label={t('account_edit_region_aria')}
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
								{isAddressExpanded ? t('account_address_hide') : t('account_address_show')}
							</button>
						</div>
						{isAddressExpanded && (
							<>
						<div className="settings-field">
							<label htmlFor="street">{isKoper ? t('address') : t('account_street_label')}</label>
							<div className="settings-field-row">
								<input
									id="street"
									className="settings-input"
									type="text"
									placeholder={isKoper ? t('address') : t('account_street_placeholder')}
									value={form.address.street}
									onChange={updateAddressField('street')}
									readOnly={!isEditable('address.street')}
								/>
								<button
									className="settings-edit-btn"
									type="button"
									onClick={() => toggleEditable('address.street')}
									aria-label={t('account_edit_street_aria')}
								>
									<i className="bi bi-pencil" />
								</button>
							</div>
						</div>
						<div className="settings-field">
							<label htmlFor="city">{isKoper ? t('account_city_label') : t('account_city_label')}</label>
							<div className="settings-field-row">
								<input
									id="city"
									className="settings-input"
									type="text"
									placeholder={isKoper ? t('account_city_placeholder') : t('account_city_placeholder')}
									value={form.address.city}
									onChange={updateAddressField('city')}
									readOnly={!isEditable('address.city')}
								/>
								<button
									className="settings-edit-btn"
									type="button"
									onClick={() => toggleEditable('address.city')}
									aria-label={t('account_edit_city_aria')}
								>
									<i className="bi bi-pencil" />
								</button>
							</div>
						</div>
						<div className="settings-field">
							<label htmlFor="regionOrState">{isKoper ? t('region') : t('account_region_state_label')}</label>
							<div className="settings-field-row">
								<input
									id="regionOrState"
									className="settings-input"
									type="text"
									placeholder={isKoper ? t('region') : t('account_region_state_placeholder')}
									value={form.address.regionOrState}
									onChange={updateAddressField('regionOrState')}
									readOnly={!isEditable('address.regionOrState')}
								/>
								<button
									className="settings-edit-btn"
									type="button"
									onClick={() => toggleEditable('address.regionOrState')}
									aria-label={t('account_edit_region_state_aria')}
								>
									<i className="bi bi-pencil" />
								</button>
							</div>
						</div>
						<div className="settings-field">
							<label htmlFor="postalCode">{isKoper ? t('postcode') : t('account_postal_code_label')}</label>
							<div className="settings-field-row">
								<input
									id="postalCode"
									className="settings-input"
									type="text"
									placeholder={isKoper ? t('postcode') : t('account_postal_code_placeholder')}
									value={form.address.postalCode}
									onChange={updateAddressField('postalCode')}
									readOnly={!isEditable('address.postalCode')}
								/>
								<button
									className="settings-edit-btn"
									type="button"
									onClick={() => toggleEditable('address.postalCode')}
									aria-label={t('account_edit_postal_aria')}
								>
									<i className="bi bi-pencil" />
								</button>
							</div>
						</div>
						<div className="settings-field">
							<label htmlFor="country">{isKoper ? t('country') : t('account_country_label')}</label>
							<div className="settings-field-row">
								<input
									id="country"
									className="settings-input"
									type="text"
									placeholder={isKoper ? t('country') : t('account_country_placeholder')}
									value={form.address.country}
									onChange={updateAddressField('country')}
									readOnly={!isEditable('address.country')}
								/>
								<button
									className="settings-edit-btn"
									type="button"
									onClick={() => toggleEditable('address.country')}
									aria-label={t('account_edit_country_aria')}
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
						{isKoper ? t('account_password_required') : t('account_password_new')}
					</label>
					<div className="settings-password-row">
						<input
							id="password"
							className="settings-input"
							type={showPassword ? 'text' : 'password'}
							placeholder={allowPasswordChange || isKoper ? t('account_password_placeholder') : t('account_password_placeholder_locked')}
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
							{showPassword ? t('account_password_hide') : t('account_password_show')}
						</button>
					</div>
					<div className="settings-inline-actions">
						<button
							className="settings-link-btn"
							type="button"
							onClick={() => setAllowPasswordChange((prev) => !prev)}
						>
							{allowPasswordChange ? t('account_password_cancel_change') : t('account_password_change')}
						</button>
					</div>
				</div>
				<button className="settings-primary-btn" type="button" onClick={handleSave} disabled={notice.type === 'saving'} aria-label={t('account_save_aria')}>
					{notice.type === 'saving' ? t('account_save_busy') : t('account_save')}
				</button>
			</div>
		</section>
	);
}

export default AccountSettings;
