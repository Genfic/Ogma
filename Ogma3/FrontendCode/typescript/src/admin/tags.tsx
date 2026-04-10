import LucidePencil from "icon:lucide:pencil";
import LucideTrash2 from "icon:lucide:trash-2";
import { Tag as TagConfig } from "@g/ctconfig";
import { DeleteApiTags, GetApiTagsAll, GetTagNamespaces, PostApiTags, PutApiTags } from "@g/paths-public";
import type { TagDto } from "@g/types-public";
import { $id } from "@h/dom";
import { getFormData } from "@h/form-helpers";
import { makeEmpty } from "@h/type-helpers";
import { createResource, For, Match, Show, Switch } from "solid-js";
import { createStore } from "solid-js/store";
import { render } from "solid-js/web";
import * as v from "valibot";

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
		return res.data;
	});

	const [form, setForm] = createStore<FormTag>(EmptyTag);

	const deleteTag = async (t: TagDto) => {
		if (confirm("Delete permanently?")) {
			const res = await DeleteApiTags(t.id, headers);
			if (!res.ok) {
				throw new Error(res.data ?? res.statusText);
			}
			await refetch();
		}
	};

	const editTag = (t: TagDto) => {
		const ns = namespaces()?.find((n) => n.name === t.namespace)?.value;
		setForm({ ...t, namespace: ns });
	};

	const cancelEdit = () => {
		setForm(makeEmpty);
	};

	const createTag = async (e: SubmitEvent) => {
		e.preventDefault();

		const [error, f] = getFormData(e, FormTagSchema);

		if (error) {
			console.error(error);
			return;
		}

		const { id, name, namespace, description } = f;

		const data = {
			name,
			namespace: (namespaces()?.find((n) => n.value === namespace)?.name ?? null) as TagDto["namespace"],
			description: description ?? null,
		};

		if (id) {
			const res = await PutApiTags({ id, ...data }, headers);
			if (!res.ok) {
				throw new Error(res.data ?? res.statusText);
			}
			await refetch();
			cancelEdit();
		} else {
			const res = await PostApiTags(data, headers);
			if (!res.ok) {
				throw new Error(res.data ?? res.statusText);
			}
			await refetch();
		}
	};

	return (
		<>
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

			<Switch>
				<Match when={namespaces.loading}>
					<button class="btn btn-primary" type="button" disabled>
						<span class="spinner-grow spinner-grow-sm" aria-hidden="true" />
						Loading...
					</button>
				</Match>
				<Match when={tags}>
					<ul class="items-list">
						<For each={tags()}>
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

render(() => <Tags />, parent);
