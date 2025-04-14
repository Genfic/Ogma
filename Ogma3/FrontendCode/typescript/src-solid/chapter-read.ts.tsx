import { customElement } from "solid-element";
import { onMount } from "solid-js";

customElement(
	"o-read",
	{
		route: String,
		chapterId: Number,
	},
	(props) => {
		onMount(() => {
			props.element.classList.add("wc-loaded");
		});

		return () => (
			<button type="button" class="read-status" aria-label="Chapter read status" data-id="@c.Id">
				<o-icon icon="lucide:eye-off"></o-icon>
			</button>
		);
	},
);
