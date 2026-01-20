/** @type {import('tailwindcss').Config} */
module.exports = {
	darkMode: 'class',
	content: ['./src/**/*.{js,jsx,ts,tsx}'],
	theme: {
		extend: {
			colors: {
				'primary': {
					100: '#b5f5e2',
					200: '#88ecd2',
					300: '#5de2c2',
					400: '#32d8b1',
					500: '#0cc988', // Your main color
					600: '#0ab479',
					700: '#099e69',
					800: '#07895b',
					main: '#0cc988',
				},
				'secondary': {
					100: '#e1daf1',
					200: '#c6bde4',
					300: '#a99cd6',
					400: '#8c7bca',
					500: '#361280', // Your secondary color
					600: '#301072',
					700: '#2a0f65',
					800: '#240d58',
					main: '#361280',
				},
			}
		},
	},
	plugins: [],
};
