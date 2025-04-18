interface DateDelta {
	years?: number;
	months?: number;
	days?: number;
	hours?: number;
	minutes?: number;
	seconds?: number;
	milliseconds?: number;
}

type Method = {
	[K in keyof Date]: Date[K] extends (...args: unknown[]) => number ? K : never;
}[keyof Date];

type Prop = keyof DateDelta;

const methods: [Method, Prop][] = [
	["getFullYear", "years"],
	["getMonth", "months"],
	["getDate", "days"],
	["getHours", "hours"],
	["getMinutes", "minutes"],
	["getSeconds", "seconds"],
	["getMilliseconds", "milliseconds"],
] as const;

type DateParams = [
	year: number,
	monthIndex: number,
	date?: number,
	hours?: number,
	minutes?: number,
	seconds?: number,
	ms?: number,
];

export const addToDate = (date: Date, delta: DateDelta = {}) => {
	return new Date(
		...(methods.map(([getter, prop]) => (date[getter] as () => number)() + (delta[prop] ?? 0)) as DateParams),
	);
};
