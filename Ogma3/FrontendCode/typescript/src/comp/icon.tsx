import { component } from "@h/web-components";
import type { ComponentType } from "solid-element";
import css from "./icon.css";

const Icon: ComponentType<{ icon: string }> = (props) => {
	return (
		<svg width="24" height="24" class="o-icon" part="icon">
			<title>{props.icon}</title>
			<use href={`/svg/spritesheet.svg#${props.icon}`} />
		</svg>
	);
};

component("o-icon", { icon: "" }, Icon, css);
