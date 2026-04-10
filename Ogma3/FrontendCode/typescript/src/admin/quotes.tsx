import LucidePencil from "icon:lucide:pencil";
import LucidePlus from "icon:lucide:plus";
import LucideTrash2 from "icon:lucide:trash-2";
import { DeleteApiQuotes, GetAllQuotes, PostApiQuotes, PostApiQuotesJson, PutApiQuotes } from "@g/paths-public";
import type { FullQuoteDto } from "@g/types-public";
import { $id } from "@h/dom";
import { getFormData } from "@h/form-helpers";
import { makeEmpty } from "@h/type-helpers";
import { createResource, For, Match, Show, Switch } from "solid-js";
import { createStore } from "solid-js/store";
import { render } from "solid-js/web";
import * as v from "valibot";
import { Dialog, type DialogApi } from "../comp/common/_dialog";

const parent = $id("quotes-app");

const headers = { RequestVerificationToken: parent.dataset.csrf ?? "" };
const isAdmin = Boolean(parent.dataset.admin);

const QuoteSchema = v.object({
	body: v.string(),
	author: v.string(),
	id: v.optional(v.pipe(v.string(), v.transform(Number), v.integer())),
});

type Quote = v.InferOutput<typeof QuoteSchema>;

const Quotes = () => {
	let json = $signal("");
	let search = $signal("");
	const [form, setForm] = createStore<Quote>({ author: "", body: "" });
	const dialogRef = $signal<DialogApi>();

	const [quotes, { mutate }] = createResource(async () => {
		const res = await GetAllQuotes(headers);
		return res.data;
	});

	const filteredQuotes = () => {
		const quot = quotes() ?? [];
		return quot.filter((q) => `${q.id} ${q.author} ${q.body}`.toLowerCase().includes(search.toLowerCase()));
	};

	const fromJson = async () => {
		const { success, issues, output } = v.safeParse(v.array(QuoteSchema), JSON.parse(json));

		if (!success) {
			console.error(issues);
			return;
		}

		const res = await PostApiQuotesJson({ quotes: output });

		if (!res.ok) {
			throw new Error(res.data ?? res.statusText);
		}

		alert(`Created ${res.data} quotes`);
	};

	const deleteQuote = async (q: FullQuoteDto) => {
		if (confirm("Delete permanently?")) {
			const res = await DeleteApiQuotes(q.id);
			if (!res.ok) {
				throw new Error(res.data ?? res.statusText);
			}
			mutate((old) => old?.filter((i) => i.id !== res.data));
		}
	};

	const openEditor = (q?: FullQuoteDto) => {
		setForm(q ?? { author: "", body: "", id: undefined });
		dialogRef?.open();
	};

	const saveQuote = async (e: SubmitEvent) => {
		e.preventDefault();

		const [error, data] = getFormData(e, QuoteSchema);

		if (error) {
			console.error(error);
			return;
		}

		const { id, author, body } = data;

		if (id) {
			const res = await PutApiQuotes({ ...data, id }, headers);
			if (!res.ok) {
				throw new Error(res.data ?? res.statusText);
			}
			mutate((old) => old?.map((v) => (v.id === id ? { id, author, body } : v)));
		} else {
			const res = await PostApiQuotes(data, headers);
			if (!res.ok) {
				throw new Error(res.data ?? res.statusText);
			}
			mutate((old) => (old ? [res.data, ...old] : [res.data]));
		}

		setForm({ body: "", author: "", id: undefined });
	};

	return (
		<>
			<form class="form" onsubmit={(e) => e.preventDefault()}>
				<Show when={isAdmin}>
					<details class="details">
						<summary>Load from Json</summary>
						<div class="o-form-group">
							<textarea
								class="o-form-control active-border"
								rows="1"
								placeholder="Json"
								onChange={(e) => {
									json = e.currentTarget.value;
								}}
							/>
							<button type="button" class="action-btn" onClick={fromJson}>
								Send
							</button>
						</div>
					</details>
				</Show>

				<div class="o-form-group">
					<input
						type="text"
						class="o-form-control active-border"
						oninput={({ currentTarget }) => {
							search = currentTarget.value;
						}}
						placeholder="Search..."
					/>
				</div>
			</form>

			<br />

			<button type="button" class="btn btn-block btn-outline-primary" onclick={() => openEditor()}>
				<LucidePlus />
				Create New
			</button>

			<Switch>
				<Match when={quotes.loading}>Loading...</Match>
				<Match when={quotes.error}>{quotes.error}</Match>
				<Match when={quotes}>
					<ul class="items-list">
						<For each={filteredQuotes()}>
							{(q) => (
								<li>
									<div class="main">
										<span class="name">{q.body}</span>
										<em class="desc">{q.author}</em>
									</div>
									<div class="actions">
										<button type="button" class="action" onclick={[deleteQuote, q]}>
											<LucideTrash2 />
										</button>
										<button type="button" class="action" onclick={[openEditor, q]}>
											<LucidePencil />
										</button>
									</div>
								</li>
							)}
						</For>
					</ul>
				</Match>
			</Switch>

			<Dialog ref={$set(dialogRef)} onClose={() => setForm(makeEmpty)}>
				<form class="content form" onsubmit={saveQuote}>
					<strong>{form?.id === null ? "Create" : "Edit"} quote</strong>

					<div class="o-form-group">
						<label for="author">Author</label>
						<input type="text" name="author" id="author" prop:value={form.author} />
					</div>

					<div class="o-form-group">
						<label for="body">Body</label>
						<textarea name="body" id="body" cols="40" rows="5" prop:value={form.body} />
					</div>

					<Show when={form.id}>
						<input type="hidden" name="id" prop:value={form.id} />
					</Show>

					<div class="o-form-group">
						<input class="btn" type="submit" value={form.id === null ? "Create" : "Save"} />
					</div>
				</form>
			</Dialog>
		</>
	);
};

render(() => <Quotes />, parent);
