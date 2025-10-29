// External imports
import React from 'react';
import clsx from 'clsx';
import AppHeader from "../nav/AppHeader";

// Internal imports


type PageProps = {
	enableHeader?: boolean;
	enableFooter?: boolean;
} & React.HTMLAttributes<HTMLDivElement>;

function Page(props: PageProps) {
	const {children, enableHeader, enableFooter, className, ...restProps} = props;
	return (
		<div
			className={clsx("app-page", className)}
			{...restProps}
		>
			{enableHeader && <AppHeader/>}
			{children}
		</div>
	);
}

export default Page;