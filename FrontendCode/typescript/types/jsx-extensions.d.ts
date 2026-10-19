// oxlint-disable import/no-unassigned-import
import "solid-js";
import "solid-labels";
import { Prettify } from "@t/utils";
import { IconProps } from "../src/comp/common/Icon";
import { QrCodeProps } from "../src/comp/qr-code";

type PropPrefixed<T> = { [K in keyof Prettify<T> as K extends string ? `prop:${K}` : never]: T[K] };

declare module "solid-js" {
	namespace JSX {
		interface IntrinsicElements {
			svg: Omit<JSX.IntrinsicElements["svg"], "part"> & { part?: string };
			"o-icon": PropPrefixed<IconProps>;
			"qr-code": PropPrefixed<QrCodeProps>;
		}
		interface DOMAttributes<T> {
			[key: `prop:${string}`]: unknown;
			[key: `attr:${string}`]: unknown;
			[key: `on:${string}`]: unknown;
			[key: `use:${string}`]: unknown;
			[key: `class:${string}`]: boolean;
			[key: `style:${string}`]: string;
		}

		interface SVGAttributes<T> {
			part?: string;
		}
	}
}
