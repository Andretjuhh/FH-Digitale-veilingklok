import React, {memo} from "react";
import clsx from "clsx";

type LoadingIconProps = React.HTMLAttributes<HTMLDivElement>;

function Spinner(props: LoadingIconProps) {
	const {className, ...rest} = props;

	return (
		<div className={clsx("custom-spinner", className)} {...rest}></div>
	);
}

export default memo(Spinner);