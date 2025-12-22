export const Avatar = ({children}: any) => (
	<div className="rounded-full bg-gray-200 h-8 w-8 grid place-content-center">
		{children}
	</div>
);
export const AvatarFallback = ({children}: any) => <>{children}</>;
