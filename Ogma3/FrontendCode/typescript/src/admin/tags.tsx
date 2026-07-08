import { Tag as TagConfig } from "@g/ctconfig";
import {
	DeleteApiTags,
	GetApiTagsAll,
	GetTagNamespaces,
	PostApiTags,
	PostApiTagsBulk,
	PutApiTags,
} from "@g/paths-public";
import type { TagDto } from "@g/types-public";
import { $id } from "@h/dom";
import { getFormData } from "@h/form-helpers";
import clsx from "clsx";
import { compact, sortBy } from "es-toolkit";
import LucidePencil from "icon:lucide:pencil";
import LucideTrash2 from "icon:lucide:trash2";
import { createResource, createSignal, For, Match, Show, Switch } from "solid-js";
import { createStore } from "solid-js/store";
import { render } from "solid-js/web";
import * as v from "valibot";
import { StyledElement } from "../comp/common/_styled";
import styles from "./tags.css";

const parent = $id("roles-app");
const headers = { RequestVerificationToken: parent.dataset.csrf ?? "" };

const FormTagSchema = v.object({
	name: v.string(),
	namespace: v.nullable(v.pipe(v.string(), v.transform(Number), v.integer())),
	description: v.nullable(v.string()),
	id: v.optional(v.pipe(v.string(), v.transform(Number), v.integer())),
});

type FormTag = v.InferOutput<typeof FormTagSchema>;

const EmptyTag = {
	id: undefined,
	name: "",
	namespace: null,
	description: null,
} satisfies FormTag;

const Tags = () => {
	const [namespaces] = createResource(async () => {
		const res = await GetTagNamespaces();
		return res.data;
	});

	const [tags, { refetch }] = createResource(async () => {
		const res = await GetApiTagsAll();
		if (!res.ok) {
			throw new Error(res.data ?? res.statusText);
		}
		return sortBy(res.data, ["namespace", "name"]);
	});

	const [form, setForm] = createStore<FormTag>(EmptyTag);
	const [bulk, setBulk] = createSignal(false);
	let jsonBulk = $signal("");
	const [errors, setErrors] = createSignal<string[]>([]);
	const [filter, setFilter] = createSignal<string | null>(null);

	const deleteTag = async (t: TagDto) => {
		if (confirm("Delete permanently?")) {
			const res = await DeleteApiTags(t.id, headers);
			if (!res.ok) {
				setErrors((e) => [...e, res.data ?? res.statusText]);
				return;
			}
			await refetch();
		}
	};

	const editTag = (t: TagDto) => {
		const ns = namespaces()?.find((n) => n.name === t.namespace)?.value;
		window.scrollTo({ top: 0, behavior: "smooth" });
		setForm({ ...t, namespace: ns });
	};

	const cancelEdit = () => {
		setForm({ name: "", namespace: null, description: null, id: undefined });
	};

	const createTag = async (e: SubmitEvent) => {
		e.preventDefault();

		const [error, f] = getFormData(e, FormTagSchema);

		if (error) {
			setErrors((e) => [...e, error.message]);
			return;
		}

		const { id, name, namespace, description } = f;

		const data = {
			name,
			namespace: (namespaces()?.find((n) => n.value === namespace)?.name ?? null) as TagDto["namespace"],
			description: description ?? null,
		};

		const res = id ? await PutApiTags({ id, ...data }, headers) : await PostApiTags(data, headers);

		if (!res.ok) {
			setErrors((e) => [...e, res.data ?? res.statusText]);
			return;
		}

		await refetch();
		cancelEdit();
	};

	const createBulkTag = async (e: SubmitEvent) => {
		e.preventDefault();

		const data = JSON.parse(jsonBulk) as Record<string, string[]>;

		const nsNames = new Set(["", ...(namespaces()?.map((n) => n.name.toLowerCase()) ?? [])]);
		const dataNamespaces = new Set(Object.keys(data).map((n) => n.toLowerCase()));

		const difference = dataNamespaces.difference(nsNames);
		if (difference.size > 0) {
			setErrors((e) => [...e, `Unknown namespace(s): ${[...difference].join(", ")}.`]);
			return;
		}

		const res = await PostApiTagsBulk({ json: jsonBulk }, headers);
		if (!res.ok) {
			setErrors((e) => [...e, res.data ?? res.statusText]);
			return;
		}

		await refetch();
		(e.currentTarget as HTMLTextAreaElement).value = "";
	};

	const changeFilter = (ns: string) => {
		if (filter() === ns) {
			setFilter(null);
		} else {
			setFilter(ns);
		}
	};

	const filtered = () =>
		tags()?.filter((t) => {
			const f = filter();
			if (f == null) return true;
			if (f === "None") return t.namespace == null;
			return t.namespace === f;
		}) ?? [];

	const active = (ns: string | null) => {
		const f = filter();

		return f ? f === ns : false;
	};

	return (
		<>
			<Show when={compact(errors()).length > 0}>
				<ul class="errors">
					{compact(errors()).map((e) => (
						<li>{e}</li>
					))}
				</ul>
			</Show>
			<Show when={!bulk()}>
				<form id="tag" class="auto" method="post" onsubmit={createTag}>
					<label for="tag-name">Name</label>
					<input
						id="tag-name"
						type="text"
						class="o-form-control"
						name="name"
						prop:value={form.name}
						minlength={TagConfig.MinNameLength}
						maxlength={TagConfig.MaxNameLength}
					/>

					<label for="tag-desc">Description</label>
					<input
						id="tag-desc"
						type="text"
						class="o-form-control"
						name="description"
						prop:value={form.description}
						maxlength={TagConfig.MaxDescLength}
					/>

					<label for="tag-namespace">Namespace</label>
					<select id="tag-namespace" class="o-form-control" name="namespace" prop:value={form.namespace}>
						<option value="" selected>
							None
						</option>
						<For each={namespaces()}>{({ value, name }) => <option value={value}>{name}</option>}</For>
					</select>

					<Show when={form.id}>
						<input type="hidden" name="id" value={form.id} />
					</Show>

					<div class="form-row">
						<button type="submit" class="btn btn-primary">
							{form.id ? "Edit" : "Add"}
						</button>
						<Show when={form.id}>
							<button type="reset" class="btn btn-secondary" onclick={cancelEdit}>
								Cancel
							</button>
						</Show>
					</div>
				</form>
			</Show>
			<Show when={bulk()}>
				<form id="tag-bulk" class="auto" method="post" onSubmit={createBulkTag}>
					<label for="tag-desc">Description</label>
					<textarea onChange={(e) => (jsonBulk = e.currentTarget.value)} rows={10} />
					<button type="submit" class="btn btn-primary">
						Submit
					</button>
				</form>
			</Show>

			<button type="button" class="btn toggle-bulk" onClick={() => setBulk((b) => !b)}>
				Toggle bulk editor
			</button>

			<Switch>
				<Match when={namespaces.loading}>
					<button class="btn btn-primary" type="button" disabled>
						<span class="spinner-grow spinner-grow-sm" aria-hidden="true" />
						Loading...
					</button>
				</Match>
				<Match when={tags}>
					<div class="tabs">
						{["None", ...(namespaces()?.map((n) => n.name) ?? [])]?.map((n) => (
							<button
								type="button"
								class={clsx("tab", active(n) && "active")}
								onClick={[changeFilter, n]}
							>
								{n}
							</button>
						))}
					</div>
					<ul class="items-list">
						<For each={filtered()}>
							{(t) => (
								<li>
									<div
										class="deco"
										style={{ "background-color": t.namespaceColor as string | undefined }}
									/>
									<div class="main">
										<h3 class="name" title={t.slug}>
											{t.name}
										</h3>
										<span class="desc">{t.description}</span>
									</div>
									<div class="actions">
										<button type="button" class="action" onclick={[deleteTag, t]}>
											<LucideTrash2 />
										</button>
										<button type="button" class="action" onclick={[editTag, t]}>
											<LucidePencil />
										</button>
									</div>
								</li>
							)}
						</For>
					</ul>
				</Match>
			</Switch>
		</>
	);
};

const S = StyledElement(Tags, styles);
render(() => <S />, parent);
