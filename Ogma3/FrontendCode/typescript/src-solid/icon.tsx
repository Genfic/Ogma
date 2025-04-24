import { type ComponentType, customElement } from "solid-element";

const Icon: ComponentType<{ icon: string }> = (props) => {
	return (
		<svg width="24" height="24" class="o-icon" part="icon">
			<title>{props.icon}</title>
			<use href={`/svg/spritesheet.svg#${props.icon}`} />
		</svg>
	);
};

customElement(
	"o-icon",
	{
		icon: "",
	},
	Icon,
);
