import { GetApiQuotesRandom as getQuote } from "@g/paths-public";
import { log } from "@h/logger";
import { customElement, noShadowDOM } from "solid-element";
import { Show, createSignal, onMount } from "solid-js";
import type { QuoteDto } from "@g/types-public";

let canLoad = true;

const loadQuote = async () => {
	if (!canLoad) {
		return JSON.parse(window.localStorage.getItem("quote")) as QuoteDto;
	}
	canLoad = false;

	const response = await getQuote();

	setTimeout(
		() => {
			canLoad = true;
		},
		Number.parseInt(response.headers.get("retry-after")) * 1000,
	);

	if (response.ok) {
		window.localStorage.setItem("quote", JSON.stringify(response.data));
		return response.data;
	}

	if (response.status === 429) {
		return JSON.parse(window.localStorage.getItem("quote")) as QuoteDto;
	}
};

customElement("quote-box", {}, (_, { element }) => {
	noShadowDOM();

	const [loading, setLoading] = createSignal(false);
	const [quote, setQuote] = createSignal<QuoteDto | null>(null);
	const [canReload, setCanReload] = createSignal(true);

	onMount(async () => {
		element.classList.add("wc-loaded");
		await load();
	});

	const spinnerIcon = () => (canReload() ? "lucide:refresh-cw" : "lucide:clock");

	const load = async () => {
		if (!canReload()) {
			return;
		}

		setLoading(true);
		const response = await getQuote();
		setLoading(false);

		if (response.ok) {
			setQuote(response.data);
			window.localStorage.setItem("quote", JSON.stringify(response.data));
		} else if (response.status === 429) {
			setQuote(JSON.parse(window.localStorage.getItem("quote")));
		} else {
			log.error(response.statusText);
		}

		setCanReload(false);
		setTimeout(
			() => {
				setCanReload(true);
			},
			Number.parseInt(response.headers.get("retry-after")) * 1000,
		);
	};

	return () => (
		<div id="quote" class="quote active-border">
			<button type="button" class="refresh" onClick={load}>
				<o-icon icon={spinnerIcon()} class={`material-icons-outlined ${loading() ? "spin" : ""}`} />
			</button>
			<Show when={quote()} fallback={<span>Loading the quote...</span>}>
				<em class="body">{quote().body}</em>
				<span class="author">{quote().author}</span>
			</Show>
		</div>
	);
});
