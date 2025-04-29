import { type ComponentType, customElement, noShadowDOM } from "solid-element";

const ChapterRead: ComponentType<{ route: string; chapterId: number }> = (props) => {
	noShadowDOM();

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
