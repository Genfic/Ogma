import { GetApiQuotesRandom as getQuote } from "@g/paths-public";
import type { QuoteDto } from "@g/types-public";
import { type ComponentType, customElement } from "solid-element";
import { Show, createResource, createSignal } from "solid-js";
import { useLocalStorage } from "@h/localStorageHook";
import css from "./quote-box.css";
import { Styled } from "./common/_styled";
import type { Empty } from "@t/utils";

const QuoteBox: ComponentType<Empty> = (_) => {
	const [canLoad, setCanLoad] = createSignal(true);
	const [getQuoteFromStore, setQuoteInStore] = useLocalStorage<QuoteDto>("quote");

	const loadQuote = async () => {
		if (!canLoad()) {
			return getQuoteFromStore();
		}
		setCanLoad(false);

		const response = await getQuote();

		setTimeout(
			() => {
				setCanLoad(true);
			},
			Number.parseInt(response.headers.get("retry-after")) * 1000,
		);

		if (response.status === 429) {
			return getQuoteFromStore();
		}

		if (response.ok) {
			setQuoteInStore(response.data);
			return response.data;
		}
	};

	const [quote, { refetch }] = createResource<QuoteDto>(loadQuote);

	const spinnerIcon = () => (canLoad() ? "lucide:refresh-cw" : "lucide:clock");

	return () => (
		<div id="quote" class="quote active-border">
			<button type="button" class="refresh" onClick={refetch}>
				<o-icon icon={spinnerIcon()} class="material-icons-outlined" classList={{ spin: quote.loading }} />
			</button>
			<Show when={quote()} fallback={<span>Loading the quote...</span>}>
				<em class="body">{quote().body}</em>
				<span class="author">{quote().author}</span>
			</Show>
		</div>
	);
};

customElement("quote-box", {}, Styled(QuoteBox, css));
