import { Shelf } from "@g/ctconfig";
import { DeleteApiShelves, GetApiShelves, PostApiShelves, PutApiShelves } from "@g/paths-public";
import { component } from "@h/web-components";
import type { ProblemDetails } from "@t/utils";
import { omit } from "es-toolkit";
import { noShadowDOM } from "solid-element";
import { createResource, For } from "solid-js";
import { InputCounter } from "./comp/common/_input-counter";
import { InputToggle } from "./comp/common/_input-toggle";
import { LucidePencil } from "./icons/LucidePencil";
import { LucideTrash2 } from "./icons/LucideTrash2";

interface Props {
	icons: string;
	userName: string;
	csrf: string;
}

type ShelfModel = {
	id?: number;
	name: string;
	description: string;
	isPublic: boolean;
	isQuickAdd: boolean;
	trackUpdates: boolean;
	color: string;
	iconId: number;
};
const EmptyShelf = {
	name: "",
	description: "",
	isPublic: false,
	isQuickAdd: false,
	trackUpdates: false,
	color: "",
	iconId: 0,
} satisfies ShelfModel;

const ManageShelves = (props: Props) => {
	noShadowDOM();

	const headers = { RequestVerificationToken: props.csrf };

	const [shelves, { refetch }] = createResource(async () => {
		const res = await GetApiShelves(props.userName, 1);
		if (!res.ok) throw res.error;
		return res.data;
	});

	let formData = $signal<ShelfModel>(EmptyShelf);

	let errors = $signal<string[]>([]);

	const handleInput = (prop: keyof ShelfModel, custom?: unknown) => (event: Event) => {
		const el = event.target as HTMLInputElement;
		formData = {
			...formData,
			[prop]: custom ?? (el.type === "checkbox" ? el.checked : el.value),
		};
	};

	const icons = () => JSON.parse(props.icons) as { Id: number; Name: string }[];

	const createOrUpdateShelf = async (e: SubmitEvent) => {
		e.preventDefault();

		const data = formData;
		if (!data) return;

		data.color = data.color.startsWith("#") ? data.color : `#${data.color}`;

		if (data.id) {
			const res = await PutApiShelves(data as Required<ShelfModel>, headers);
			if (res.ok) {
				refetch();
			} else if (res.status === 400) {
				errors = Object.values((res as unknown as ProblemDetails).errors).flat() as string[];
			} else {
				throw new Error(res.error);
			}
		} else {
			const res = await PostApiShelves(omit(data, ["id"]), headers);
			if (res.ok) {
				refetch();
			} else if (res.status === 400) {
				errors = Object.values((res as unknown as ProblemDetails).errors).flat() as string[];
			} else {
				throw new Error(res.error);
			}
		}
		clear();
	};

	const clear = () => {
		formData = EmptyShelf;
	};

	const editShelf = (shelf: ShelfModel) => {
		formData = shelf;
	};

	const deleteShelf = async (id: number) => {
		if (confirm("Deleting a bookshelf is irreversible. Are you sure?")) {
			const res = await DeleteApiShelves(id);
			if (!res.ok) throw new Error(res.error);
			await refetch();
		}
	};

	return (
		<>
			<div class="errors">
				<ul>
					<For each={errors}>{(e) => <li>{e}</li>}</For>
				</ul>
			</div>

			<form class="form" id="category" onsubmit={createOrUpdateShelf}>
				<div class="form-row">
					<InputCounter
						label="Name"
						min={Shelf.MinNameLength}
						max={Shelf.MaxNameLength}
						validateMsg="The {0} should be between {2} and {1} characters"
						value={formData.name}
						onInput={handleInput("name")}
					/>

					<InputCounter
						label="Description"
						max={Shelf.MaxDescriptionLength}
						validateMsg="The {0} should be less than {1} characters"
						value={formData.description}
						onInput={handleInput("description")}
					/>
				</div>

				<div class="form-row">
					<div class="o-form-group keep-size">
						<label for="Color">Color</label>
						<input
							name="Color"
							type="color"
							class="form-control"
							value={formData.color}
							onInput={handleInput("color")}
						/>
					</div>

					<InputToggle
						label="Quick Access"
						value={formData.isQuickAdd}
						onChange={handleInput("isQuickAdd")}
					/>

					<InputToggle label="Public" value={formData.isPublic} onChange={handleInput("isPublic")} />

					<InputToggle
						label="Track Updates"
						value={formData.trackUpdates}
						onChange={handleInput("trackUpdates")}
					/>
				</div>

				<div class="form-row">
					<fieldset class="o-form-group">
						<legend>Icon</legend>

						<div class="select-group">
							<For each={icons()}>
								{(i) => {
									const id = `icon-${i.Id}`;
									return (
										<>
											<input
												type="radio"
												name="iconId"
												id={id}
												value={i.Id}
												checked={formData.iconId === i.Id}
												onchange={handleInput("iconId", i.Id)}
											/>
											<label for={id}>
												<o-icon icon={i.Name} />
											</label>
										</>
									);
								}}
							</For>
						</div>
					</fieldset>
				</div>
				{(() => {
					const data = formData;
					if (data?.id) {
						return <input type="hidden" name="id" value={data.id} />;
					}
					return null;
				})()}

				<div class="form-group">
					<button type="submit" class="btn btn-primary">
						{formData.id ? "Update" : "Create"}
					</button>
					<button type="reset" class="btn" onclick={[$set(formData), EmptyShelf]}>
						Clear
					</button>
				</div>
			</form>

			<hr />

			<ul class="shelves generic-list">
				<For each={shelves()}>
					{(s) => (
						<li class="bookshelf-card calm" style={{ "--shelf-color": s.color ?? undefined }}>
							<o-icon class="ico" icon={s.iconName ?? ""} />
							<span class="name">{s.name}</span>
							<span class="desc">{s.description} &nbsp;</span>
							<div class="count">{s.storiesCount}</div>
							<div class="buttons">
								<button
									type="button"
									class="btn"
									title="Delete bookshelf"
									onclick={[deleteShelf, s.id]}
								>
									<LucideTrash2 />
								</button>
								<button type="button" class="btn" title="Edit bookshelf" onclick={[editShelf, s]}>
									<LucidePencil />
								</button>
							</div>
						</li>
					)}
				</For>
			</ul>
		</>
	);
};

component(
	"o-shelves-management",
	{
		icons: "",
		userName: "",
		csrf: "",
	},
	ManageShelves,
);
