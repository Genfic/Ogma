import convert from "convert";

export class Stopwatch {
	start: number;

	constructor() {
		this.start = Bun.nanoseconds();
	}

	lap(precision: number): { time: string; unit: string };
	lap(): number;
	lap(precision?: number | undefined): number | { time: string; unit: string } {
		const current = Bun.nanoseconds();
		const time = current - this.start;
		this.start = current;
		if (precision) {
			const { quantity, unit } = convert(time, "ns").to("best");
			return { time: quantity.toFixed(precision), unit };
		}
		return time;
	}
}
