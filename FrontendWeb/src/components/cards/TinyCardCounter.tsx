import React from 'react';
import clsx from "clsx";
import {joinClsx} from "../../utils/classPrefixer";
import CountUp from "../../libraries/react-bits/CountUp";

type TinyCardCounterProps = {
	className?: string;
	icon: string;
	title: string;
	description?: string;
	counter: number;
	counterPrefix?: string;
};

function TinyCardCounter(props: TinyCardCounterProps) {
	const {icon, className, title, counter, description, counterPrefix} = props;
	return (
		<div className={clsx("tiny-card", className)}>
			<div className="tiny-card-hd">
				<span>{icon && <i className={clsx("tiny-card-icon", joinClsx(className, 'icon'), "bi", icon)}/>}</span>
				<p className={clsx("tiny-card-title", joinClsx(className, 'title'))}>{title}</p>
			</div>

			{description && <p className={clsx("tiny-card-description", joinClsx(className, 'description'))}>{description}</p>}

			<div className={clsx("tiny-card-data", joinClsx(className, 'data'))}>
				<CountUp
					from={0}
					to={counter}
					separator=","
					direction="up"
					duration={1}
					className={clsx("tiny-card-data-txt", joinClsx(className, 'data-txt'))}
				/>

				<p className={clsx("tiny-card-data-txt", joinClsx(className, 'data-txt'))}>
					{counterPrefix}
				</p>
			</div>
		</div>

	);
}

export default TinyCardCounter;