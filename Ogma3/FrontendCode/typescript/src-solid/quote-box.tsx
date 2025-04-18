import { GetApiQuotesRandom as getQuote } from "@g/paths-public";
import type { QuoteDto } from "@g/types-public";
import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { Show, createResource, createSignal, onMount } from "solid-js";

const QuoteBox: ComponentType<null> = (_, { element }) => {
	noShadowDOM();

	onMount(() => {
		element.classList.add("wc-loaded");
	});

	const [canLoad, setCanLoad] = createSignal(true);

	const loadQuote = async () => {
		if (!canLoad()) {
			return JSON.parse(window.localStorage.getItem("quote")) as QuoteDto;
		}
		setCanLoad(false);

		const response = await getQuote();

		setTimeout(
			() => {
				setCanLoad(true);
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

	const [quote, { refetch }] = createResource<QuoteDto>(() => loadQuote(), {});

	const spinnerIcon = () => (canLoad() ? "lucide:refresh-cw" : "lucide:clock");

	return () => (
		<div id="quote" class="quote active-border">
			<button type="button" class="refresh" onClick={refetch}>
				<o-icon icon={spinnerIcon()} class={`material-icons-outlined ${quote.loading ? "spin" : ""}`} />
			</button>
			<Show when={quote()} fallback={<span>Loading the quote...</span>}>
				<em class="body">{quote().body}</em>
				<span class="author">{quote().author}</span>
			</Show>
		</div>
	);
};

customElement("quote-box", null, QuoteBox);
