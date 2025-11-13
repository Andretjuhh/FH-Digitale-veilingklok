import i18n from 'i18next';
import {initReactI18next, useTranslation} from 'react-i18next';

export const resources = {
	nl: {
		translation: {
			// General texts
			veilingklok: 'Veilingklok',
			growers: 'Kwekers',
			buyer: 'Kopers',
			language: 'Taal',
			services: 'Diensten',
			flowers: 'Bloemen',
			transactions: 'Transacties',

			// Auth texts
			login: 'Inloggen',
			get_Started: 'Meld je aan',
			logout: 'Uitloggen',

			//Login page texts
			welcome_back: 'Welkom terug',
			sign_in_message: 'Meld u alstublieft aan bij uw account',
			forgot_password: 'Wachtwoord vergeten?',
			create_account: 'Account aanmaken',
			password: 'Wachtwoord',
			email: 'E-mailadres',
		

			// Home page Texts
			what_is_flori_clock: "Wat is FloriClock?",
			what_is_flori_clock_description: "FloriClock is een digitaal veilingplatform dat kwekers en bloemisten samenbrengt om bloemen en planten efficiënt te verhandelen. Ons doel is om de veilinghandel in bloemen en planten te vereenvoudigen en te optimaliseren door middel van een gebruiksvriendelijke online marktplaats.",
			how_it_works: "Hoe werkt het?",
			flower_types: "Bloemsoorten",
			flower_types_description: "Ontdek een breed scala aan bloemsoorten die beschikbaar zijn op ons platform. Van rozen tot tulpen, vind precies wat je zoekt.",
			contact_us: 'Contacteer ons',
			welcome_title: 'Welkom bij FloriClock',
			welcome_description: `Bloemen en planten verkopen en kopen vanop nu nog makkelijker met FloriClock!
			Een innovatief platform dat kwekers en bloemisten samenbrengt om bloemen en planten efficiënt te verhandelen.`,
			welcome_cta_text: `Begin vandaag nog met het ontdekken van ons platform. Meld je aan of log in om toegang te krijgen tot onze diensten.`,
			flowers_description: 'Verschillende bloemsoorten',
			growers_description: 'Groot aantal kwekers aangesloten',
			transactions_description: 'Succesvolle transacties dagelijks',
			what_do_growers: 'Plaatsen van hun producten en beheren van hun aanbod',
			veilingmeesters: 'Veilingmeesters',
			what_do_veilingmeesters: 'Beheren van veilingen en zorgen voor een vlotte afhandeling',
			what_do_buyer: 'Zoeken en bieden op bloemen en planten van hun keuze',
			what_is_veilingklok: 'Een geavanceerd systeem dat de tijd en volgorde van biedingen regelt tijdens veilingen',

			// Header and Footer texts
			dashboard: 'Dashboard',
			manage_account: 'Account beheren',
			settings: 'Instellingen',
			orders: 'Orders',
			back: "Terug",
			

			// Error page texts
			go_back_home: 'Ga terug naar home',
			something_went_wrong: 'Something went wrong.',
			unexpected_happened: 'An unexpected error occurred. Please try again later.',

			// Aria labels and alt texts
			alt_error_bug_picture: 'Error bug image',
			back_button_aria: "Knop om terug te gaan",
			login_button_aria: "Knop om in te loggen",
			forgot_password_aria: "Wachtwoord vergeten link",
			create_account_aria: "Account aanmaken link",

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