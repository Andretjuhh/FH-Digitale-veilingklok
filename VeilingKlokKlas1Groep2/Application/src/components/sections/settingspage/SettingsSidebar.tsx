function SettingsSidebar({ onChange, active }: { onChange: (section: string) => void; active: string }) {
	return (
		<div className="sidebar">
			<button onClick={() => onChange('account')}>Account</button>
			<button onClick={() => onChange('privacy')}>Privacy</button>
			<button onClick={() => onChange('notifications')}>Notifications</button>
		</div>
	);
}
export default SettingsSidebar;
