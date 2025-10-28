import i18n from 'i18next';
import {initReactI18next} from 'react-i18next';

export const resources = {
	nl: {
		translation: {
			what_is_flori_clock: "Wat is FloriClock?",
			how_it_works: "Hoe werkt het?",
			services: 'Diensten',
			contact_us: 'Contacteer ons',
		},
	},
	en: {
		translation: {},
	},
	es: {
		translation: {},
	},
	fr: {
		translation: {},
	},
}
export type LocalizationTexts = keyof typeof resources['en']['translation'];
export type SupportedLanguages = keyof typeof resources;

i18n.use(initReactI18next).init({
	resources,
	lng: 'en',// if you're using a language detector, do not define the lng option
	fallbackLng: 'en',
	debug: false,
	interpolation: {escapeValue: false},
}).then(null);

