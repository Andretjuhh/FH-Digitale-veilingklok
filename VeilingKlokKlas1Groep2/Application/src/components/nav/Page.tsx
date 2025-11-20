// External imports
import React from 'react';
import clsx from 'clsx';

// Internal imports
import AppHeader from "../nav/AppHeader";
import AppFooter from "../nav/AppFooter";

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
			{enableFooter && <AppFooter/>}
		</div>
	);
}

export default Page;