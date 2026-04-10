import LucideEyeOff from "icon:lucide:eye-off";
import LucidePencil from "icon:lucide:pencil";
import LucideTrash2 from "icon:lucide:trash-2";
import { DeleteApiRatings, GetApiRatings, PostApiRatings, PutApiRatings } from "@g/paths-public";
import { $id } from "@h/dom";
import { expose } from "@h/expose";
import { getFormData } from "@h/form-helpers";
import { checkboxBool, strippedHexColor } from "@h/valibot-schemas";
import { createResource, For, Show } from "solid-js";
import { createStore } from "solid-js/store";
import { render } from "solid-js/web";
import * as v from "valibot";

const parent = $id("ratings-app");

const headers = { RequestVerificationToken: parent.dataset.csrf ?? "" };

const RatingSchema = v.object({
	name: v.string(),
	description: v.string(),
	order: v.pipe(v.string(), v.transform(Number), v.minValue(0), v.maxValue(255), v.integer()),
	blacklistedByDefault: checkboxBool,
	color: strippedHexColor,
	id: v.optional(v.pipe(v.string(), v.transform(Number), v.integer())),
});

type Rating = v.InferOutput<typeof RatingSchema>;

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
			return res.data;
		},
		{ initialValue: [] },
	);

	expose({ ratings });

	const [form, setForm] = createStore<Rating>(EmptyRating);

	const deleteRating = async (id: number) => {
		const res = await DeleteApiRatings(id, headers);
		if (res.ok) {
			refetch();
		} else {
			throw new Error(res.data ?? res.statusText);
		}
	};

	const createRating = async (e: SubmitEvent) => {
		e.preventDefault();

		const [error, data] = getFormData(e, RatingSchema);
		if (error) {
			console.error(error);
			return;
		}

		if (data.id) {
			const res = await PutApiRatings({ ...data, id: data.id }, headers);
			if (res.ok) {
				refetch();
			} else {
				throw new Error(res.data ?? res.statusText);
			}
		} else {
			const res = await PostApiRatings(data, headers);
			if (res.ok) {
				refetch();
			} else {
				throw new Error(res.data ?? res.statusText);
			}
		}

		setForm(EmptyRating);
	};

	const openForEdit = (rating: Rating) => {
		console.log(rating);
		setForm(rating);
	};

	return (
		<>
			<form class="auto" onSubmit={createRating}>
				<label for="name">Name</label>
				<input class="o-form-control" name="name" prop:value={form.name} />

				<label for="description">Description</label>
				<input class="o-form-control" name="description" prop:value={form.description} />

				<label for="color">Color</label>
				<input type="color" class="o-form-control" name="color" prop:value={`#${form.color}`} />

				<label for="order">Order</label>
				<input type="number" step="1" min="0" class="o-form-control" name="order" prop:value={form.order} />

				<label for="blacklistedByDefault">Blacklisted by default</label>
				<input
					type="checkbox"
					class="o-form-control"
					name="blacklistedByDefault"
					prop:checked={form.blacklistedByDefault}
				/>

				<Show when={form.id}>
					<input type="hidden" name="id" value={form.id} />
				</Show>

				<input class="o-form-control btn" type="submit" value="Submit" />
				<button type="reset">Clear</button>
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
