import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { onMount } from "solid-js";

const ChapterRead: ComponentType<{ route: string; chapterId: number }> = (props, { element }) => {
	noShadowDOM();

	onMount(() => {
		element.classList.add("wc-loaded");
	});

	return (
		<button type="button" class="read-status" aria-label="Chapter read status" data-id="@c.Id">
			<o-icon icon="lucide:eye-off" />
		</button>
	);
};

customElement(
	"o-read",
	{
		route: "",
		chapterId: 0,
	},
	ChapterRead,
);
