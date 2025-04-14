import { addToDate } from "@h/date-helpers";
import { EU, iso8601 } from "@h/tinytime-templates";
import { customElement } from "solid-element";
import { createSignal, onCleanup, onMount } from "solid-js";

customElement(
	"o-clock",
	{
		date: {
			type: (value) => new Date(value),
			default: new Date(),
		},
	},
	(props) => {
		const [date, setDate] = createSignal(props.date);

		onMount(() => {
			props.element.classList.add("wc-loaded");

			// Add styles to shadow root
			const style = document.createElement("style");
			style.textContent = `
      time {
        font-family: "Courier New", Courier, monospace;
        letter-spacing: -2px;
        margin: auto 0;
      }
    `;
			props.element.shadowRoot.appendChild(style);

			// Setup the timer
			const interval = setInterval(() => {
				setDate(addToDate(date(), { seconds: 1 }));
			}, 1000);

			onCleanup(() => clearInterval(interval));
		});

		return () => (
			<time class="timer" datetime={iso8601.render(date())} title="Server time">
				{EU.render(date())}
			</time>
		);
	},
);
