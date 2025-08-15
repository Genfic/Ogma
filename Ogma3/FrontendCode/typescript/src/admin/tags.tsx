import { Tag as TagConfig } from "@g/ctconfig";
import { DeleteApiTags, GetApiTagsAll, GetTagNamespaces, PostApiTags, PutApiTags } from "@g/paths-public";
import type { TagDto } from "@g/types-public";
import { $id } from "@h/dom";
import { makeEmpty } from "@h/type-helpers";
import { createResource, For, Match, Show, Switch } from "solid-js";
import { createStore } from "solid-js/store";
import { render } from "solid-js/web";
import { LucidePencil } from "../icons/LucidePencil";
import { LucideTrash2 } from "../icons/LucideTrash2";

const parent = $id("roles-app");
const headers = { RequestVerificationToken: parent.dataset.csrf ?? "" };

const Tags = () => {
	const [namespaces] = createResource(async () => {
		const res = await GetTagNamespaces();
		if (!res.ok) throw res.error;
		return res.data;
	});

	const [tags, { refetch }] = createResource(async () => {
		const res = await GetApiTagsAll();
		if (!res.ok) throw res.error;
		return res.data;
	});

	const [form, setForm] = createStore<Partial<Omit<TagDto, "namespace"> & { namespace: number }>>({});

	const deleteTag = async (t: TagDto) => {
		if (confirm("Delete permanently?")) {
			const res = await DeleteApiTags(t.id, headers);
			if (!res.ok) throw res.error;
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

	const createTag = async (e: Event) => {
		e.preventDefault();

		const { id, name, namespace, description } = form;

		if (!name) return;

		const data = {
			name,
			namespace: (namespaces()?.find((n) => n.value === namespace)?.name ?? null) as TagDto["namespace"],
			description: description ?? null,
		};

		if (id) {
			const res = await PutApiTags({ id, ...data }, headers);
			if (!res.ok) throw res.error;
			await refetch();
			cancelEdit();
		} else {
			const res = await PostApiTags(data, headers);
			if (!res.ok) throw res.error;
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
					value={form.name ?? ""}
					minlength={TagConfig.MinNameLength}
					maxlength={TagConfig.MaxNameLength}
					oninput={({ target }) => setForm("name", target.value)}
				/>

				<label for="tag-desc">Description</label>
				<input
					id="tag-desc"
					type="text"
					class="o-form-control"
					value={form.description ?? ""}
					maxlength={TagConfig.MaxDescLength}
					oninput={({ target }) => setForm("description", target.value)}
				/>

				<label for="tag-namespace">Namespace</label>
				<select
					id="tag-namespace"
					class="o-form-control"
					value={form.namespace ?? ""}
					oninput={({ target }) => setForm("namespace", Number.parseInt(target.value))}
				>
					<option value="" selected>
						None
					</option>
					<For each={namespaces()}>{({ value, name }) => <option value={value}>{name}</option>}</For>
				</select>

				<input type="hidden" value={form.id ?? ""} />

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
						<span class="spinner-grow spinner-grow-sm" role="status" aria-hidden="true" />
						Loading...
					</button>
				</Match>
				<Match when={tags}>
					<ul class="items-list">
						<For each={tags()}>
							{(t) => (
								<li>
									<div class="deco" style={{ "background-color": t.namespaceColor }} />
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
