import i18n from 'i18next';
import {initReactI18next, useTranslation} from 'react-i18next';

export const resources = {
	nl: {
		translation: {
			what_is_flori_clock: "Wat is FloriClock?",
			how_it_works: "Hoe werkt het?",
			flower_types: "Bloemsoorten",
			services: 'Diensten',
			contact_us: 'Contacteer ons',
			welcome_title: 'Welkom bij FloriClock',
			welcome_description: `Bloemen en planten verkopen en kopen vanop nu nog makkelijker met FloriClock!
			Een innovatief platform dat kwekers en bloemisten samenbrengt om bloemen en planten efficiënt te verhandelen.`,
			welcome_cta_text: `Begin vandaag nog met het ontdekken van ons platform. Meld je aan of log in om toegang te krijgen tot onze diensten.`,
			get_Started: 'Meld je aan',
			login: 'Inloggen',
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
i18n.use(initReactI18next).init({
	resources,
	lng: 'nl',// if you're using a language detector, do not define the lng option
	fallbackLng: 'nl',
	debug: false,
	interpolation: {escapeValue: false},
}).then(null);

export type LocalizationResources = typeof resources['nl'] //keyof typeof resources['nl']['translation'];
export type LocalizationTexts = keyof typeof resources['nl']['translation'];
export type SupportedLanguages = keyof typeof resources;
export {useTranslation};