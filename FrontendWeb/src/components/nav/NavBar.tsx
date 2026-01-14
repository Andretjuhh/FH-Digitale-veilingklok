import React from 'react';
import {useRootContext} from "../contexts/RootContext";
import {useLocation} from "react-router-dom";


type Props = {
	pages: { key: string, name: string, location: string, icon: string, strict?: boolean }[];
}

const NavBar = (props: Props) => {
	const {pages} = props;
	const {navigate,} = useRootContext();
	const location = useLocation();
	const routeName = location.pathname;
	console.log('Current Route:', routeName);

	return (
		<div className="navbar-tabs" style={{'--tabs-count': pages.length} as React.CSSProperties}>
			{pages.map((page, index) => (
				<React.Fragment key={page.key}>
					<input
						id={`radio-${page.key}`}
						type="radio"
						name="navbar-tabs"
						className="navbar-tab-input"
						defaultChecked={page.strict ?? true ? routeName === page.location : routeName.startsWith(page.location)}
						onChange={() => navigate(page.location)}
					/>

					<label
						className={`navbar-tab-label ${routeName === page.location ? 'navbar-tab-label-active' : ''}`}
						htmlFor={`radio-${page.key}`}
						style={{'--tab-index': index} as React.CSSProperties}
					>
						<i className={"navbar-tab-icon bi " + page.icon}></i>
						{page.name}
					</label>
				</React.Fragment>
			))}
			<span className="navbar-tabs-glider"></span>
		</div>
	);
}

export default NavBar;
