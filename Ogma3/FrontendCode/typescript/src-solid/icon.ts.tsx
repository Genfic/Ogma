import { customElement } from "solid-element";

customElement(
	"o-icon",
	{
		icon: "",
	},
	(props) => {
		return (
			<svg width="24" height="24" class="o-icon" part="icon">
				<title>${props.icon}</title>
				<use href={`/svg/spritesheet.svg#${props.icon}`} />
			</svg>
		);
	},
);
