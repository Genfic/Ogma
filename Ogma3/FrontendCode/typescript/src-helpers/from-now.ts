// https://github.com/lukeed/fromnow/blob/master/src/index.js

const MIN = 60e3;
const HOUR = MIN * 60;
const DAY = HOUR * 24;
const YEAR = DAY * 365;
const MONTH = DAY * 30;

interface Options {
	/** Maximum number of segments to return */
	max?: number;
	/** Appends "ago" or "from now" */
	suffix?: boolean;
	/** Joins last two segments with "and" */
	and?: boolean;
	/** Returns segments even if they're 0 */
	zero?: boolean;
}

export const fromNow = (date: string, opts: Options = {}): string => {
	const del = new Date(date).getTime() - Date.now();
	const abs = Math.abs(del);

	if (abs < MIN) return "just now";

	const periods = {
		year: abs / YEAR,
		month: (abs % YEAR) / MONTH,
		day: (abs % MONTH) / DAY,
		hour: (abs % DAY) / HOUR,
		minute: (abs % HOUR) / MIN,
	};

	let val: number;
	const keep = [];
	const max = opts.max || MIN; // large number

	for (const period in periods) {
		if (keep.length < max) {
			val = Math.floor(periods[period]);
			if (val || opts.zero) {
				keep.push(`${val} ${val === 1 ? period : `${period}s`}`);
			}
		}
	}

	let k = keep.length;
	let sep = ", ";

	if (k > 1 && opts.and) {
		if (k === 2) sep = " ";
		keep[--k] = `and ${keep[k]}`;
	}

	let val2 = keep.join(sep);

	if (opts.suffix) {
		val2 += del < 0 ? " ago" : " from now";
	}

	return val2;
};
