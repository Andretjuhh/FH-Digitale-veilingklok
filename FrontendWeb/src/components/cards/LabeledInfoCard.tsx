import React from 'react';
import clsx from "clsx";
import {joinClsx} from "../../utils/classPrefixer";

type LabeledInfoCardProps = {
	title: string;
	value: string | number;
	className?: string;
	icon: React.ReactNode;
	color?: string;
}

function LabeledInfoCard(props: LabeledInfoCardProps) {
	const {color} = props;


	return (
		<div className={clsx('labeled-info-detail-item', props.className)}>
			<div className={clsx('labeled-info-detail-bg', joinClsx(props.className, 'bg'), color)}>
				{props.icon}
			</div>
			<div className={clsx('labeled-info-detail-texts', joinClsx(props.className, 'texts'))}>
				<span className={clsx('labeled-info-detail-label', joinClsx(props.className, 'label'))}>
					{props.title}
				</span>
				<span className={clsx('labeled-info-detail-value', joinClsx(props.className, 'value'))}>
					{props.value}
				</span>
			</div>
		</div>
	);
}

export default LabeledInfoCard;