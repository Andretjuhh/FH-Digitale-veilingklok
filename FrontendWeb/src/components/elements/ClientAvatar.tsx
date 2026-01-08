import React from "react";

const getRandomColor = (name: string) => {
	const colors = ['#EF4444', '#F59E0B', '#10B981', '#3B82F6', '#6366F1', '#8B5CF6', '#EC4899'];
	let hash = 0;
	for (let i = 0; i < name.length; i++) {
		hash = name.charCodeAt(i) + ((hash << 5) - hash);
	}
	return colors[Math.abs(hash) % colors.length];
};

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