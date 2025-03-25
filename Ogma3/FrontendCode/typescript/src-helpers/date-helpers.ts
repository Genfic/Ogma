interface DateDelta {
	years?: number;
	months?: number;
	days?: number;
	hours?: number;
	minutes?: number;
	seconds?: number;
	milliseconds?: number;
}

export const addToDate = (date: Date, delta: DateDelta = {}) => {
	return new Date(
		date.getFullYear() + (delta.years ?? 0),
		date.getMonth() + (delta.months ?? 0),
		date.getDate() + (delta.days ?? 0),
		date.getHours() + (delta.hours ?? 0),
		date.getMinutes() + (delta.minutes ?? 0),
		date.getSeconds() + (delta.seconds ?? 0),
		date.getMilliseconds() + (delta.milliseconds ?? 0),
	);
};
