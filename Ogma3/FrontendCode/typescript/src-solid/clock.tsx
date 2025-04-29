import { addToDate } from "@h/date-helpers";
import { Styled } from "./common/_styled";
import { EU, iso8601 } from "@h/tinytime-templates";
import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { createSignal, onCleanup, onMount } from "solid-js";
import css from "./clock.css";

const Clock: ComponentType<{ date: string }> = (props) => {
	noShadowDOM();
	const [date, setDate] = createSignal(new Date(props.date));

	onMount(() => {
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
	Styled(Clock, css),
);
