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
			for: 'voor',

			// Account types
			Koper: 'Koper',
			Kweker: 'Kweker',
			Veilingmeester: 'Veilingmeester',

			// Auth texts
			login: 'Inloggen',
			get_Started: 'Meld je aan',
			logout: 'Uitloggen',

			// Auth State messages
			logging_in: 'Logging in...',
			logged_in: 'Logged in successfully',
			creating_account: 'Creating account...',
			logging_out: 'Logging out...',
			account_created: 'Account created successfully',
			account_created_error: 'Account creation failed',
			account_created_error_message: 'An error occurred while creating your account. Please try again later.',
			account_created_error_message_technical: 'An error occurred while creating your account. Please contact support.',
			account_created_error_message_technical_technical: 'An error occurred while creating your account. Please contact support.',

			//Login and register page texts
			login_title: 'Log in',
			welcome_back: 'Welkom terug',
			sign_in_message: 'Meld u alstublieft aan bij uw account',
			forgot_password: 'Wachtwoord vergeten?',
			create_account: 'Account aanmaken',
			password: 'Wachtwoord',
			email: 'E-mailadres',
			address: 'Adres',
			postcode: 'Postcode',
			region: 'Regio',
			company_name: 'Bedrijfsnaam',
			kvk_number: 'KVK-Nummer',
			phonenumber: 'Telefoonnummer',
			authorisation_code: 'Autorisatiecode',
			country: 'Land',
			step: 'Stap',
			of: 'van',
			first_name: 'Voornaam',
			last_name: 'Achternaam',
			previous: 'Vorige',
			next: 'Volgende',
			login_message: 'Inloggen?',

			// Home page Texts
			what_is_flori_clock: 'Wat is FloriClock?',
			what_is_flori_clock_description: 'FloriClock is een digitaal veilingplatform dat kwekers en bloemisten samenbrengt om bloemen en planten efficiënt te verhandelen. Ons doel is om de veilinghandel in bloemen en planten te vereenvoudigen en te optimaliseren door middel van een gebruiksvriendelijke online marktplaats.',
			how_it_works: 'Hoe werkt het?',
			flower_types: 'Bloemsoorten',
			flower_types_description: 'Ontdek een breed scala aan bloemsoorten die beschikbaar zijn op ons platform. Van rozen tot tulpen, vind precies wat je zoekt.',
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

			// Koper Dashboard texts
			koper_dashboard: 'Mijn Dashboard',
			koper_dashboard_sub: 'Overzicht van je profiel en acties',
			koper_supplier: 'Aanvoerder',
			koper_avr: 'avr nr',
			koper_product: 'Producten',
			koper_country: 'land',
			koper_stems_per_bundle: 'aantal stelen per bos',
			koper_stock: 'Voorraad: {{count}} stuks',
			koper_qty_input_aria: 'Aantal te kopen stelen',
			koper_buy: 'Koop product',
			koper_max_stock: 'Max voorraad',
			koper_upcoming: 'Volgende',
			koper_bundle_label: 'bos',
			koper_footer_about_title: 'Over FloriClock',
			koper_footer_about_line1: 'Digitale veiling voor bloemen en planten.',
			koper_footer_about_line2: 'Gebouwd door studenten - demo omgeving.',
			koper_footer_product_title: 'Product',
			koper_footer_live: 'Live veiling',
			koper_footer_history: 'Prijsgeschiedenis',
			koper_footer_favorites: 'Favorieten',
			koper_footer_resources_title: 'Resources',
			koper_footer_docs: 'Documentatie',
			koper_footer_faq: 'Veelgestelde vragen',
			koper_footer_status: 'Status',
			koper_footer_contact_title: 'Contact',
			koper_footer_support: 'Service & support',
			koper_footer_form: 'Contactformulier',
			koper_footer_locations: 'Locaties',
			refresh: 'Vernieuwen',
			no_products_available: 'Geen producten beschikbaar',

			// Veilingmeester dashboard texts
			vm_title: 'Veilingmeester',
			vm_tab_auction: 'Veiling',
			vm_tab_history: 'History',
			vm_section_subtitle: 'Beheer de veiling en volg de producten.',
			vm_button_open_auction: 'Open veiling',
			vm_button_end_auction: 'Eindig veiling',
			vm_button_start_auction: 'Start veiling',
			vm_alert_no_products: 'Er zijn geen producten om te veilen.',
			vm_alert_open_failed: 'Openen van veiling mislukt (zie console)',
			vm_alert_min_price_invalid: 'Vul eerst een geldige minimumprijs in.',
			vm_queue_load_error: 'Producten ophalen mislukt.',
			vm_empty_no_products: 'Geen producten beschikbaar.',
			vm_clock_placeholder: 'Klok',
			vm_label_duration_seconds: 'Duur (seconden)',
			vm_label_min_price: 'Minimumprijs (veilingmeester)',
			vm_section_other_products: 'Overige producten',
			vm_loading_products: 'Producten laden...',
			vm_stock_label: 'Voorraad',
			vm_stock_value: 'Voorraad: {{count}}',
			vm_max_price_label: 'Maximumprijs (kweker)',
			vm_price_not_set: 'Nog niet gezet',
			vm_history_empty: 'Nog geen veilinghistorie beschikbaar.',
			vm_history_unknown_date: 'Onbekende datum',
			vm_history_total_revenue_label: 'Totale omzet',
			vm_history_start_label: 'Start',
			vm_history_end_label: 'Einde',
			vm_history_products_label: 'Producten',
			vm_history_revenue_label: 'Omzet',
			vm_history_sold_products_title: 'Verkochte producten',
			vm_history_unknown_buyer: 'Onbekende koper',
			vm_history_piece_count: '{{count}} stuks',
			vm_history_products_count: '{{count}} producten',
			vm_history_auction_title: 'Veiling {{date}}',
			vm_demo_buyer: 'Demo koper',

			// Veilingmeester aria labels
			vm_tab_auction_aria: 'Tabblad veiling',
			vm_tab_history_aria: 'Tabblad historie',
			vm_button_open_auction_aria: 'Knop om veiling te openen',
			vm_button_end_auction_aria: 'Knop om veiling te eindigen',
			vm_button_start_auction_aria: 'Knop om veiling te starten',
			vm_input_duration_aria: 'Invoerveld voor veilingduur in seconden',
			vm_input_min_price_aria: 'Invoerveld voor minimumprijs',
			vm_history_summary_aria: 'Toon details voor veiling op {{date}}',
			vm_queue_name_1: 'Rode Rozen Premium',
			vm_queue_desc_1: 'Dieprode rozen van topkwaliteit',
			vm_queue_company_1: 'Kwekerij Bloemenhof',
			vm_queue_name_2: 'Witte Lelies',
			vm_queue_desc_2: 'Verse witte lelies, grote knoppen',
			vm_queue_company_2: 'Lelie Centrum BV',
			vm_queue_name_3: 'Zonnebloem XL',
			vm_queue_desc_3: 'Grote zonnebloemen met stevige stelen',
			vm_queue_company_3: 'Zon & Co',
			vm_queue_name_4: 'Tulpen Mix',
			vm_queue_desc_4: 'Mix van voorjaarskleuren, premium selectie',
			vm_queue_company_4: 'Tulipa Holland',
			vm_queue_name_5: 'Orchidee Phalaenopsis',
			vm_queue_desc_5: 'Bloeiend en rijk vertakt',
			vm_queue_company_5: 'Orchid World',

			// Header and Footer texts
			dashboard: 'Dashboard',
			manage_account: 'Account beheren',
			settings: 'Instellingen',
			orders: 'Orders',
			back: 'Terug',

			// Error page texts
			go_back_home: 'Ga terug naar home',
			something_went_wrong: 'Something went wrong.',
			unexpected_happened: 'An unexpected error occurred. Please try again later.',
			email_required_error: 'E-mail vereist',
			email_invalid_error: 'E-mail klopt niet (example@email.com)',
			password_required_error: 'Wachtwoord vereist',
			required: 'vereist',

			// Aria labels and alt texts
			alt_error_bug_picture: 'Error bug image',
			email_aria: 'Input veld om email in te vullen',
			first_name_aria: 'Input veld om voornaam in te vullen',
			last_name_aria: 'Input veld om achternaam in te vullen',
			password_aria: 'Input veld om wachtwoord in te vullen',
			back_button_aria: 'Knop om terug te gaan',
			login_button_aria: 'Knop om in te loggen',
			forgot_password_aria: 'Wachtwoord vergeten link',
			create_account_aria: 'Account aanmaken link',
			progress_bar: 'Aanmeld progressie: stap',
			login_message_aria: 'Link om terug naar inlog scherm te gaan',
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
};
i18n
	.use(initReactI18next)
	.init({
		resources,
		lng: 'nl', // if you're using a language detector, do not define the lng option
		fallbackLng: 'nl',
		debug: false,
		interpolation: {escapeValue: false},
	})
	.then(null);

type LocalizationResources = (typeof resources)['nl']; //keyof typeof resources['nl']['translation'];
type LocalizationTexts = keyof (typeof resources)['nl']['translation'];
type SupportedLanguages = keyof typeof resources;
export {useTranslation, LocalizationTexts, LocalizationResources, SupportedLanguages};
