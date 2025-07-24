import { DeleteApiQuotes, GetAllQuotes, PostApiQuotes, PostApiQuotesJson, PutApiQuotes } from "@g/paths-public";
import type { FullQuoteDto, QuoteDto } from "@g/types-public";
import { $id } from "@h/dom";
import { omit } from "es-toolkit";
import { createResource, For, Match, Show, Switch } from "solid-js";
import { createStore } from "solid-js/store";
import { render } from "solid-js/web";
import { Dialog, type DialogApi } from "../comp/common/_dialog";

const parent = $id("quotes-app");

const headers = { RequestVerificationToken: parent.dataset.csrf ?? "" };
const isAdmin = Boolean(parent.dataset.admin);

const isQuoteDto = (value: unknown): value is QuoteDto => {
	if (value === null || typeof value !== "object") return false;

	if (!("body" in value) || typeof value.body !== "string") return false;
	if (!("author" in value) || typeof value.author !== "string") return false;

	return true;
};

const isQuoteDtoArray = (value: unknown): value is QuoteDto[] => {
	if (!Array.isArray(value)) return false;

	return value.every(isQuoteDto);
};

const Quotes = () => {
	let json = $signal("");
	let search = $signal("");
	const [form, setForm] = createStore<Partial<FullQuoteDto>>({});
	let dialogRef = $signal<DialogApi>();

	const [quotes, { mutate }] = createResource(async () => {
		const res = await GetAllQuotes(headers);
		if (!res.ok) {
			throw res.error;
		}

		return res.data;
	});

	const filteredQuotes = () => {
		const quot = quotes() ?? [];
		return quot.filter((q) => `${q.id} ${q.author} ${q.body}`.toLowerCase().includes(search.toLowerCase()));
	};

	const fromJson = async () => {
		const obj = JSON.parse(json);
		if (!isQuoteDtoArray(obj)) return;

		const res = await PostApiQuotesJson({ quotes: obj });

		if (!res.ok) {
			throw res.error;
		}

		alert(`Created ${res.data} quotes`);
	};

	const deleteQuote = async (q: FullQuoteDto) => {
		if (confirm("Delete permanently?")) {
			const res = await DeleteApiQuotes(q.id);
			if (!res.ok) {
				throw res.error;
			}
			mutate((old) => old?.filter((i) => i.id !== res.data));
		}
	};

	const openEditor = (q?: FullQuoteDto) => {
		setForm(q ?? {});
		dialogRef?.open();
	};

	const saveQuote = async (e: SubmitEvent) => {
		e.preventDefault();

		const { id, author, body } = form;

		if (!body || !author) return;

		if (id) {
			const res = await PutApiQuotes(form as FullQuoteDto, headers);
			if (!res.ok) throw res.error;
			mutate((old) => old?.map((v) => (v.id === id ? { id, author, body } : v)));
		} else {
			const res = await PostApiQuotes(omit(form as FullQuoteDto, ["id"]), headers);
			if (!res.ok) throw res.error;
			mutate((old) => (old ? [res.data, ...old] : [res.data]));
		}

		setForm({});
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
				<o-icon icon="lucide:plus" />
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
											<o-icon icon="lucide:trash-2" />
										</button>
										<button type="button" class="action" onclick={[openEditor, q]}>
											<o-icon icon="lucide:pencil" />
										</button>
									</div>
								</li>
							)}
						</For>
					</ul>
				</Match>
			</Switch>

			<Dialog ref={(t) => (dialogRef = t)}>
				<form class="content form" onsubmit={saveQuote}>
					<strong>{form?.id === null ? "Create" : "Edit"} quote</strong>

					<div class="o-form-group">
						<label for="author">Author</label>
						<input
							type="text"
							name="author"
							id="author"
							value={form?.author ?? ""}
							oninput={({ target }) => setForm("author", target.value)}
						/>
					</div>

					<div class="o-form-group">
						<label for="body">Body</label>
						<textarea
							name="body"
							id="body"
							cols="40"
							rows="5"
							value={form?.body ?? ""}
							oninput={({ target }) => setForm("body", target.value)}
						/>
					</div>

					<div class="o-form-group">
						<input class="btn" type="submit" value={form?.id === null ? "Create" : "Save"} />
					</div>
				</form>
			</Dialog>
		</>
	);
};

render(() => <Quotes />, parent);
