import { addToDate } from "@h/date-helpers";
import { addStyle } from "@h/jsx-wc-style";
import { EU, iso8601 } from "@h/tinytime-templates";
import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { createSignal, onCleanup, onMount } from "solid-js";
import { minifyCss } from "@h/minify.macro" with { type: "macro" };

const Clock: ComponentType<{ date: string }> = (props, { element }) => {
	noShadowDOM();
	const [date, setDate] = createSignal(new Date(props.date));

	onMount(() => {
		element.classList.add("wc-loaded");
		addStyle(
			element,
			minifyCss(`
				.time {
					font-family: "Courier New", Courier, monospace;
					letter-spacing: -2px;
					margin: auto 0;
				}
			`),
		);

		// Set up the timer
		const interval = setInterval(() => {
			setDate(addToDate(date(), { seconds: 1 }));
		}, 1000);

		onCleanup(() => clearInterval(interval));
	});

	return (
		<time class="timer" datetime={iso8601.render(date())} title="Server time">
			{EU.render(date())}
		</time>
	);
};

customElement(
	"o-clock",
	{
		date: "",
	},
	Clock,
);
