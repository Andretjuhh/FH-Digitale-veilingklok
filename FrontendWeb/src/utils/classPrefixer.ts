function internalJoinClsx(base: string, prefix: string): string {
	// Clean base: remove trailing non-alphanumeric chars
	const cleanedBase = base.replace(/[^a-zA-Z0-9]+$/, "");

	// Clean prefix: remove leading non-alphanumeric chars
	const cleanedPrefix = prefix.replace(/^[^a-zA-Z0-9]+/, "");

	if (!cleanedPrefix) return cleanedBase; // avoid trailing '-'

	return `${cleanedBase}-${cleanedPrefix}`;
}

export function joinClsx(base: string | undefined, prefix: string): string | undefined {
	if (!base) return undefined;

	// Check if base have space and turn it into an array
	const baseParts = base.split(" ").filter((part) => part.trim().length > 0);

	// If multiple parts, join each part with the prefix recursively
	if (baseParts.length) {
		const joinedParts = baseParts.map((part) => internalJoinClsx(part, prefix));
		return joinedParts.filter((part) => part !== undefined).join(" ");
	}

	return undefined
}