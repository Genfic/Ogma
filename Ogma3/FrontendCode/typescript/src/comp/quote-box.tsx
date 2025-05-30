import { GetApiQuotesRandom as getQuote } from "@g/paths-public";
import type { QuoteDto } from "@g/types-public";
import { type ComponentType, customElement } from "solid-element";
import { Show, createResource, createSignal } from "solid-js";
import { useLocalStorage } from "@h/localStorageHook";
import css from "./quote-box.css";
import { Styled } from "./common/_styled";
import type { Empty } from "@t/utils";

const QuoteBox: ComponentType<Empty> = (_) => {
	const [nextFetchTime, setNextFetchTime] = createSignal(0);
	const [getQuoteFromStore, setQuoteInStore] = useLocalStorage<QuoteDto>("quote");

	const canLoad = () => Date.now() >= nextFetchTime();

	const loadQuote = async () => {
		const stored = getQuoteFromStore();

		if (!canLoad()) {
			if (stored) {
				return stored;
			}
			throw new Error("Rate limited with no fallback");
		}

		const response = await getQuote();

		if (response.ok) {
			setQuoteInStore(response.data);
			return response.data;
		}

		if (response.status === 429) {
			setNextFetchTime(Number.parseInt(response.headers.get("Retry-After") ?? "0") * 1000);
			if (stored) {
				return stored;
			}
			throw new Error("Rate limited with no fallback");
		}
		throw new Error(`Quote fetch error: ${response.error}`);
	};

	const [quote, { refetch }] = createResource<QuoteDto>(loadQuote);

	const spinnerIcon = () => (canLoad() ? "lucide:refresh-cw" : "lucide:clock");

	return () => (
		<div id="quote" class="quote active-border">
			<button type="button" class="refresh" onClick={refetch}>
				<o-icon icon={spinnerIcon()} class="material-icons-outlined" classList={{ spin: quote.loading }} />
			</button>
			<Show when={quote()} fallback={<span>Loading the quote...</span>}>
				{(q) => (
					<>
						<em class="body">{q().body}</em>
						<span class="author">{q().author}</span>
					</>
				)}
			</Show>
		</div>
	);
};

customElement("quote-box", {}, Styled(QuoteBox, css));
