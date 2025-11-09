import { GetApiQuotesRandom as getQuote } from "@g/paths-public";
import type { QuoteDto } from "@g/types-public";
import { useLocalStorage } from "@h/localStorageHook";
import { component } from "@h/web-components";
import type { Empty } from "@t/utils";
import type { ComponentType } from "solid-element";
import { createResource, Show } from "solid-js";
import { LucideClock } from "../icons/LucideClock";
import { LucideRefreshCw } from "../icons/LucideRefreshCw";
import css from "./quote-box.css";

const QuoteBox: ComponentType<Empty> = (_) => {
	let canFetch = $signal(true);
	const [getQuoteFromStore, setQuoteInStore] = useLocalStorage<QuoteDto>("quote");

	const loadQuote = async () => {
		const stored = getQuoteFromStore();

		if (!canFetch) {
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
			const nextFetchTime = Number.parseInt(response.headers.get("Retry-After") ?? "0", 10) * 1000;
			canFetch = false;
			window.setTimeout(() => {
				canFetch = true;
			}, nextFetchTime);
			if (stored) {
				return stored;
			}
			throw new Error("Rate limited with no fallback");
		}
		throw new Error(`Quote fetch error: ${response.error}`);
	};

	const [quote, { refetch }] = createResource<QuoteDto>(loadQuote);

	return () => (
		<div id="quote" class="quote active-border">
			<button type="button" class="refresh" onClick={refetch} title="Get new quote">
				{canFetch ? <LucideRefreshCw /> : <LucideClock />}
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

component("quote-box", {}, QuoteBox, css);
