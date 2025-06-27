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
	}
}
