import i18n from 'i18next';

export const resources = {
	en: {
		translation: {},
	},
	es: {
		translation: {},
	},
	nl: {
		translation: {},
	},
	fr: {
		translation: {},
	},
}
export type LocalizationTexts = keyof typeof resources['en']['translation'];

i18n.init({
	resources,
	lng: 'en',// if you're using a language detector, do not define the lng option
	fallbackLng: 'en',
	debug: false,
	interpolation: {escapeValue: false},
});

