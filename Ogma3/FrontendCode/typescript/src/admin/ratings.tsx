import { DeleteApiRatings, GetApiRatings, PostApiRatings, PutApiRatings } from "@g/paths-public";
import type { RatingApiDto2 as RatingApiDto } from "@g/types-public";
import { $id } from "@h/dom";
import { createNormalizedForm } from "@h/normalized-form";
import { omit } from "es-toolkit";
import { createResource, For } from "solid-js";
import { render } from "solid-js/web";
import type { Optional, Required } from "utility-types";
import { LucideEyeOff } from "../icons/LucideEyeOff";
import { LucidePencil } from "../icons/LucidePencil";
import { LucideTrash2 } from "../icons/LucideTrash2";

const parent = $id("ratings-app");

const headers = { RequestVerificationToken: parent.dataset.csrf ?? "" };

type Rating = Optional<RatingApiDto, "id">;
const EmptyRating = {
	id: undefined,
	name: "",
	description: "",
	order: 0,
	blacklistedByDefault: false,
	color: "",
} satisfies Rating;

const Ratings = () => {
	const [ratings, { refetch }] = createResource(
		async () => {
			const res = await GetApiRatings();
			if (!res.ok) {
				throw new Error(res.error);
			}
			return res.data;
		},
		{ initialValue: [] },
	);

	const { formData, handleInput, handleSubmit, setFormData } = createNormalizedForm<Rating>(EmptyRating);

	const deleteRating = async (id: number) => {
		const res = await DeleteApiRatings(id, headers);
		if (res.ok) {
			refetch();
		} else {
			throw new Error(res.error);
		}
	};

	const createRating = async (e: SubmitEvent) => {
		e.preventDefault();

		const data = handleSubmit(e);
		if (!data) return;

		if (data.id) {
			const res = await PutApiRatings(data as Required<Rating, "id">, headers);
			if (res.ok) {
				refetch();
			} else {
				throw new Error(res.error);
			}
		} else {
			const res = await PostApiRatings(omit(data, ["id"]), headers);
			if (res.ok) {
				refetch();
			} else {
				throw new Error(res.error);
			}
		}
		clear();
	};

	const openForEdit = (rating: Rating) => {
		setFormData(rating);
	};

	const clear = () => {
		setFormData(EmptyRating);
	};

	return (
		<>
			<form class="auto" onSubmit={createRating}>
				<label for="name">Name</label>
				<input class="o-form-control" name="name" value={formData().name} onInput={handleInput("name")} />

				<label for="description">Description</label>
				<input
					class="o-form-control"
					name="description"
					value={formData().description}
					onInput={handleInput("description")}
				/>

				<label for="color">Color</label>
				<input
					type="color"
					class="o-form-control"
					name="color"
					value={`#${formData().color}`}
					onInput={handleInput("color")}
				/>

				<label for="order">Order</label>
				<input
					type="number"
					step="1"
					min="0"
					class="o-form-control"
					name="order"
					value={formData().order}
					onInput={handleInput("order")}
				/>

				<label for="blacklistedByDefault">Blacklisted by default</label>
				<input
					type="checkbox"
					class="o-form-control"
					name="blacklistedByDefault"
					checked={formData().blacklistedByDefault}
					onInput={handleInput("blacklistedByDefault")}
				/>

				<input class="o-form-control btn" type="submit" value="Submit" />
				<button type="button" onClick={clear}>
					Clear
				</button>

				{(() => {
					const data = formData();
					if (data?.id) {
						return <input type="hidden" name="id" value={data.id} />;
					}
					return null;
				})()}
			</form>

			<ul class="items-list">
				<For each={ratings()}>
					{(rating) => (
						<li>
							<div
								class="deco"
								style={{
									"background-color": `#${rating.color}`,
								}}
							/>
							<div class="main">
								<h3 class="name">
									{rating.name}
									{rating.blacklistedByDefault && <LucideEyeOff />}
								</h3>
								<div class="desc">{rating.description}</div>
							</div>
							<div class="actions">
								<button type="button" class="action" onclick={[deleteRating, rating.id]}>
									<LucideTrash2 />
								</button>
								<button type="button" class="action" onclick={[openForEdit, rating]}>
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

render(() => <Ratings />, parent);
