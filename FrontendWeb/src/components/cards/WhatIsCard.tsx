import React from 'react';

type WhatIsCardProps = {
	icon: string
	title: string
	description: string
};

function WhatIsCard(props: WhatIsCardProps) {
	const {icon, title, description} = props;

	return (
		<div className={'app-what-is-card'}>
			<div className={'app-what-is-card-h'}>
				<i className={`app-what-is-card-icon bi ${icon}`}/>
				<h3 className={'app-what-is-card-title'}>{title}</h3>
			</div>
			<span className={'app-what-is-card-txt'}>{description}</span>
		</div>
	);
}

export default WhatIsCard;