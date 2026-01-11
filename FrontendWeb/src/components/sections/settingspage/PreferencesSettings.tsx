import React, { useEffect, useMemo, useState } from 'react';

import { useRootContext } from '../../contexts/RootContext';
import { SupportedLanguages } from '../../../controllers/services/localization';
import { LocalStorageService } from '../../../controllers/services/localStorage';

type ThemeMode = 'light' | 'dark';

type LanguageOption = {
	id: SupportedLanguages;
	label: string;
};

function PreferencesSettings() {
	const { t, changeLanguage, languageCode, account } = useRootContext();
	const storageKey = useMemo(
		() => `theme:${account?.email ?? 'guest'}`,
		[account?.email]
	);
	const [themeMode, setThemeMode] = useState<ThemeMode>('light');

	useEffect(() => {
		const stored = LocalStorageService.getItem<string>(storageKey);
		if (stored === 'dark' || stored === 'light') {
			setThemeMode(stored);
			return;
		}
		const dataTheme = document.documentElement.getAttribute('data-theme');
		setThemeMode(dataTheme === 'dark' ? 'dark' : 'light');
	}, [storageKey]);

	useEffect(() => {
		const root = document.documentElement;
		root.setAttribute('data-theme', themeMode);
		root.classList.toggle('dark', themeMode === 'dark');
		document.body.classList.toggle('dark', themeMode === 'dark');
		LocalStorageService.setItem(storageKey, themeMode);
	}, [storageKey, themeMode]);

	const languages = useMemo<LanguageOption[]>(
		() => [
			{ id: 'nl', label: t('language_dutch') },
			{ id: 'en', label: t('language_english') },
			{ id: 'fr', label: t('language_french') },
			{ id: 'es', label: t('language_spanish') },
			{ id: 'de', label: t('language_german') },
		],
		[t]
	);

	return (
		<section className="settings-panel">
			<header className="settings-panel-header">
				<h2>{t('settings_preferences_title')}</h2>
				<p>{t('settings_preferences_subtitle')}</p>
			</header>
			<div className="settings-panel-body">
				<div className="settings-field">
					<label>{t('settings_theme_label')}</label>
					<label className="settings-toggle settings-toggle-slider">
						<input
							type="checkbox"
							checked={themeMode === 'dark'}
							aria-label={t('settings_theme_aria')}
							onChange={(event) => setThemeMode(event.target.checked ? 'dark' : 'light')}
						/>
						<span className="settings-toggle-track" aria-hidden="true">
							<span className="settings-toggle-thumb" />
						</span>
						<span className="settings-toggle-label">
							{themeMode === 'dark' ? t('settings_theme_dark') : t('settings_theme_light')}
						</span>
					</label>
				</div>
				<div className="settings-field">
					<label htmlFor="preferences-language">{t('settings_language_label')}</label>
					<select id="preferences-language" className="settings-input" value={languageCode} onChange={(event) => changeLanguage(event.target.value as SupportedLanguages)}>
						{languages.map((lang) => (
							<option key={lang.id} value={lang.id}>
								{lang.label}
							</option>
						))}
					</select>
				</div>
			</div>
		</section>
	);
}

export default PreferencesSettings;
