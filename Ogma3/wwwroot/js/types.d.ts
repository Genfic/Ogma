// tinytime.d.ts

declare module "@angius/tinytime" {
	type TinyTime = {
		render: (date: Date) => string;
	};

	export type TinyTimeOptions = {
		padHours?: boolean;
		padDays?: boolean;
		padMonth?: boolean;
	};

	function tinytime(template: string, options?: TinyTimeOptions): TinyTime;
}
