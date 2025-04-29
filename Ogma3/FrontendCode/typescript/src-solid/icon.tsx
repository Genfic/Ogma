import { type ComponentType, customElement } from "solid-element";
import { Styled } from "./common/_styled";
import css from "./icon.css";

const Icon: ComponentType<{ icon: string }> = (props) => {
	return (
		<svg width="24" height="24" class="o-icon" part="icon">
			<title>{props.icon}</title>
			<use href={`/svg/spritesheet.svg#${props.icon}`} />
		</svg>
	);
};

customElement("o-icon", { icon: "" }, Styled(Icon, css));
