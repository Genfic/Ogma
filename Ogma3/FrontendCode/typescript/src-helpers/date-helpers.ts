import { toZonedTime } from "date-fns-tz";

let tzCache: string | null = null;

/**
 * Converts a date to the current timezone
 * @param date
 */
export const toCurrentTimezone = (date: Date) => {
	if (!tzCache) {
		tzCache = document.documentElement.getAttribute("data-timezone") ?? "UTC";
	}
	return toZonedTime(date, tzCache);
};
