import React from "react";
import {getRandomColor} from "../../utils/standards";


export const ClientAvatar = ({name}: { name: string }) => {
	const initials = name
		.split(' ')
		.map((n) => n[0])
		.join('')
		.toUpperCase()
		.substring(0, 2);
	const color = getRandomColor(name);
	return (
		<div style={{backgroundColor: color}} className="w-8 h-8 rounded-full flex items-center justify-center text-white text-xs font-bold mr-3 overflow-hidden shrink-0">
			{initials}
		</div>
	);
};