// External imports
import React, {useLayoutEffect, useRef} from 'react';
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
	const pageRef = useRef<HTMLElement>(null);

	useLayoutEffect(() => {
		if (pageRef.current) {
			document.documentElement.style.backgroundColor = window.getComputedStyle(pageRef.current).backgroundColor;
		}
		return () => {
			document.documentElement.style.backgroundColor = '';
		};
	});

	return (
		<article ref={pageRef} className={clsx('app-page', className)} {...restProps}>
			{enableHeader && <AppHeader className={headerClassName} slideAnimation={enableHeaderAnimation}/>}
			{children}
			{enableFooter && <AppFooter/>}
		</article>
	);
}

export default Page;
