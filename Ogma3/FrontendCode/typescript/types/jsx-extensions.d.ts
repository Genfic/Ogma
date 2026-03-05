import "solid-js";
import "solid-labels";

declare module "solid-js" {
	namespace JSX {
		interface IntrinsicElements {
			svg: Omit<JSX.IntrinsicElements["svg"], "part"> & { part?: string };
			"o-icon": {
				icon: string;
			} & JSX.HTMLAttributes<HTMLElement>;
		}
		interface DOMAttributes<T> {
			[key: `prop:${string}`]: unknown;
			[key: `attr:${string}`]: unknown;
			[key: `on:${string}`]: unknown;
			[key: `use:${string}`]: unknown;
			[key: `class:${string}`]: boolean;
			[key: `style:${string}`]: string;
		}
	}
}
