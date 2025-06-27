import { addToDate } from "@h/date-helpers";
import { EU, iso8601 } from "@h/tinytime-templates";
import { type ComponentType, customElement } from "solid-element";
import { onCleanup, onMount } from "solid-js";
import css from "./clock.css";
import { Styled } from "./common/_styled";

const Clock: ComponentType<{ date: string }> = (props) => {
	let date = $signal(new Date(props.date));

	onMount(() => {
		// Set up the timer
		const interval = setInterval(() => {
			date = addToDate(date, { seconds: 1 });
		}, 1000);

		onCleanup(() => clearInterval(interval));
	});

	return (
		<time class="timer" datetime={iso8601.render(date)} title="Server time">
			{EU.render(date)}
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
