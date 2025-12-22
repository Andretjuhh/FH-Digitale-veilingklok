export const Button = ({children, ...props}: any) => (
	<button
		className="px-3 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
		{...props}
	>
		{children}
	</button>
);
