// External imports
import React from 'react';
import clsx from 'clsx';

// Internal imports
import AppHeader from '../nav/AppHeader';
import AppFooter from '../nav/AppFooter';

type PageProps = {
	enableHeader?: boolean;
	enableFooter?: boolean;
	enableHeaderAnimation?: boolean;
	headerClassName?: string;
} & React.HTMLAttributes<HTMLElement>;

function Page(props: PageProps) {
	const {children, enableHeader, enableFooter, enableHeaderAnimation = true, className, headerClassName, ...restProps} = props;
	return (
		<article className={clsx('app-page', className)} {...restProps}>
			{enableHeader && <AppHeader className={headerClassName} slideAnimation={enableHeaderAnimation}/>}
			{children}
			{enableFooter && <AppFooter/>}
		</article>
	);
}

export default Page;
