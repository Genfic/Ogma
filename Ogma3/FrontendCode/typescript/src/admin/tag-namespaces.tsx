import { TagNamespace } from "@g/ctconfig";
import {
	DeleteApiTagnamespaces,
	GetApiTagnamespaces,
	PostApiTagnamespaces,
	PutApiTagnamespaces,
} from "@g/paths-public";
import type { TagNamespaceDto } from "@g/types-public";
import { $id } from "@h/dom";
import { getFormData } from "@h/form-helpers";
import { strippedHexColor } from "@h/valibot-schemas";
import LucidePencil from "icon:lucide:pencil";
import LucideTags from "icon:lucide:tags";
import LucideTrash2 from "icon:lucide:trash-2";
import { createResource, createSignal, For, Match, Show, Switch } from "solid-js";
import { createStore } from "solid-js/store";
import { render } from "solid-js/web";
import * as v from "valibot";
import { StyledElement } from "../comp/common/_styled";
import styles from "./tag-namespaces.css";

const parent = $id("tag-namespaces");
const headers = { RequestVerificationToken: parent.dataset.csrf ?? "" };

const Ns = TagNamespace;

const FormNamespaceSchema = v.object({
	name: v.pipe(v.string(), v.trim(), v.minLength(Number(Ns.MinNameLength)), v.maxLength(Number(Ns.MaxNameLength))),
	slug: v.nullable(v.pipe(v.string(), v.trim(), v.maxLength(Number(Ns.MaxSlugLength)))),
	alias: v.nullable(v.pipe(v.string(), v.trim(), v.maxLength(Number(Ns.MaxAliasLength)))),
	color: v.nullable(strippedHexColor),
	description: v.nullable(v.pipe(v.string(), v.trim(), v.maxLength(Number(Ns.MaxDescLength)))),
	id: v.optional(v.pipe(v.string(), v.transform(Number), v.integer())),
});

type FormNamespace = v.InferOutput<typeof FormNamespaceSchema>;

type TagNamespaceView = Omit<TagNamespaceDto, "color"> & { color: string | null };

const DefaultColor = "#8c37f4";

/**
 * Must return a new object each call. `createStore` mutates its seed object in place, so
 * handing the same instance back to the setter would make every field compare equal and the
 * reset would silently do nothing.
 */
const emptyNamespace = (): FormNamespace => ({
	id: undefined,
	name: "",
	slug: null,
	alias: null,
	color: DefaultColor,
	description: null,
});

const TagNamespaces = () => {
	const [namespaces, { refetch }] = createResource(async () => {
		const res = await GetApiTagnamespaces();
		if (!res.ok) {
			throw new Error(res.statusText);
		}
		return res.data.map((ns) => ({ ...ns, color: ns.color === null ? null : `#${ns.color}` }));
	});

	const [form, setForm] = createStore<FormNamespace>(emptyNamespace());
	const [errors, setErrors] = createSignal<string[]>([]);

	const cancelEdit = () => {
		setForm(emptyNamespace());
	};

	const editNamespace = (ns: TagNamespaceView) => {
		window.scrollTo({ top: 0, behavior: "smooth" });
		setForm({
			id: ns.id,
			name: ns.name,
			slug: ns.slug,
			alias: ns.alias,
			color: ns.color ?? DefaultColor,
			description: ns.description,
		});
	};

	const deleteNamespace = async (ns: TagNamespaceView) => {
		const warning =
			ns.tagCount > 0
				? `Delete "${ns.name}"?\n\nIts ${ns.tagCount} tag(s) are NOT deleted - they become un-namespaced.`
				: `Delete "${ns.name}"?`;

		if (!confirm(warning)) {
			return;
		}

		const res = await DeleteApiTagnamespaces(ns.id, headers);
		if (!res.ok) {
			setErrors((e) => [...e, res.statusText]);
			return;
		}

		if (form.id === ns.id) {
			cancelEdit();
		}

		await refetch();
	};

	const saveNamespace = async (ev: SubmitEvent) => {
		ev.preventDefault();

		const [error, f] = getFormData(ev, FormNamespaceSchema);

		if (error) {
			setErrors((e) => [...e, error.message]);
			console.error(error);
			return;
		}

		const { id, name, slug, alias, color, description } = f;

		// The API treats a null slug as "derive it from the name".
		const data = {
			name,
			slug: slug ? slug : null,
			alias: alias ? alias : null,
			color: color ?? null,
			description: description ?? null,
		};

		const res = id
			? await PutApiTagnamespaces({ id, ...data }, headers)
			: await PostApiTagnamespaces(data, headers);

		if (!res.ok) {
			setErrors((e) => [...e, res.data ?? res.statusText]);
			return;
		}

		await refetch();
		cancelEdit();
	};

	return (
		<>
			<form id="tag-namespace" class="auto" method="post" onsubmit={saveNamespace}>
				<label for="tns-name">Name</label>
				<input
					id="tns-name"
					type="text"
					name="name"
					class="o-form-control"
					minlength={Ns.MinNameLength}
					maxlength={Ns.MaxNameLength}
					required
					prop:value={form.name}
				/>

				<label for="tns-slug">Slug</label>
				<input
					id="tns-slug"
					type="text"
					name="slug"
					class="o-form-control"
					maxlength={Ns.MaxSlugLength}
					placeholder="Derived from the name if left empty"
					prop:value={form.slug ?? ""}
				/>

				<label for="tns-alias">Alias</label>
				<input
					id="tns-alias"
					type="text"
					name="alias"
					class="o-form-control"
					maxlength={Ns.MaxAliasLength}
					prop:value={form.alias ?? ""}
				/>

				<label for="tns-color">Color</label>
				<input
					id="tns-color"
					type="color"
					name="color"
					class="o-form-control"
					prop:value={form.color ?? DefaultColor}
				/>

				<label for="tns-description">Description</label>
				<textarea
					id="tns-description"
					name="description"
					class="o-form-control"
					maxlength={Ns.MaxDescLength}
					prop:value={form.description ?? ""}
				/>

				<Show when={form.id}>
					<input type="hidden" name="id" prop:value={form.id} />
				</Show>

				<div class="form-row">
					<button type="submit" class="btn btn-primary">
						{form.id ? "Edit" : "Add"}
					</button>
					<Show when={form.id}>
						<button class="btn btn-secondary" type="button" onclick={cancelEdit}>
							Cancel
						</button>
					</Show>
				</div>
			</form>

			<Show when={errors().length > 0}>
				<div class="validation-summary validation-summary-errors">
					<ul>
						<For each={errors()}>{(e) => <li>{e}</li>}</For>
					</ul>
				</div>
			</Show>

			<br />

			<Switch>
				<Match when={namespaces.loading}>
					<button class="btn btn-primary" type="button" disabled>
						<span class="spinner-grow spinner-grow-sm" aria-hidden="true" />
						Loading...
					</button>
				</Match>
				<Match when={namespaces.error}>{String(namespaces.error)}</Match>
				<Match when={namespaces}>
					<ul class="items-list namespaces">
						<For each={namespaces()}>
							{(ns) => (
								<li>
									<div class="deco" style={ns.color ? { "background-color": ns.color } : undefined} />
									<div class="main">
										<h3 class="name">
											<span class="label">{ns.name}</span>
											<code class="slug" title="URL slug">
												{ns.slug}
											</code>
											<Show when={ns.alias}>
												<code class="alias" title="Short alias">
													{ns.alias}
												</code>
											</Show>
										</h3>
										<Show when={ns.description}>
											<p class="desc">{ns.description}</p>
										</Show>
									</div>
									<span class="count" title="Tags in this namespace">
										<LucideTags />
										{ns.tagCount}
									</span>
									<div class="actions">
										<button type="button" class="action" onclick={[deleteNamespace, ns]}>
											<LucideTrash2 />
										</button>
										<button type="button" class="action" onclick={[editNamespace, ns]}>
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

const S = StyledElement(TagNamespaces, styles);
render(() => <S />, parent);
