import React, { useState } from 'react';
import Button from '../../buttons/Button';

function AccountSettings() {
	const [email, setEmail] = useState('user@example.com');

	return (
		<div className="settings-content p-6">
			<h1 className="text-2xl font-semibold">Account Settings</h1>
			<div className="form-field-group">
				<div className="flex flex-col gap-1">
					<label className="font-medium">Email</label>
					<input type="email" value={email} onChange={(e) => setEmail(e.target.value)} className="border p-2 rounded-xl" />
					<Button>Change Email</Button>
				</div>
				<div className="flex flex-col gap-1">
					<label className="font-medium">Password</label>
					<input type="password" value="************" readOnly className="border p-2 rounded-xl" />
					<Button>Change Password</Button>
				</div>
			</div>
		</div>
	);
}
export default AccountSettings;
