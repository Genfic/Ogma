let tzCache: string | null = null;

/**
 * Converts a date to the current timezone
 * @param date
 */
export const toCurrentTimezone = (date: Date) => {
	if (!tzCache) {
		tzCache = document.documentElement.dataset.timezone ?? "UTC";
	}
	return convertTimeZone(date, tzCache);
};

export const convertTimeZone = (date: Date, timezone: string) => {
	const formatter = new Intl.DateTimeFormat("en-US", {
		timeZone: timezone,
		year: "numeric",
		month: "numeric",
		day: "numeric",
		hour: "numeric",
		minute: "numeric",
		second: "numeric",
		hour12: false,
	});

	const map: Record<string, number> = {};
	for (const part of formatter.formatToParts(date)) {
		if (part.type === "literal") continue;
		map[part.type] = Number.parseInt(part.value, 10);
	}

	return new Date(map.year, map.month - 1, map.day, map.hour, map.minute, map.second);
};
