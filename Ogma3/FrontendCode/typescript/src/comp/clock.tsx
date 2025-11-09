import { addToDate } from "@h/date-helpers";
import { EU, iso8601 } from "@h/tinytime-templates";
import { component } from "@h/web-components";
import type { ComponentType } from "solid-element";
import { onCleanup, onMount } from "solid-js";
import css from "./clock.css";

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

component(
	"o-clock",
	{
		date: "",
	},
	Clock,
	css,
);
