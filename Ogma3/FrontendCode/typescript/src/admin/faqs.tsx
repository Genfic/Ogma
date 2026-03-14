import { DeleteApiFaqs, GetApiFaqs, PostApiFaqs, PutApiFaqs } from "@g/paths-public";
import type { FaqDto } from "@g/types-public";
import { $id } from "@h/dom";
import { getFormData } from "@h/form-helpers";
import { createResource, For, Show } from "solid-js";
import { createStore } from "solid-js/store";
import { render } from "solid-js/web";
import * as v from "valibot";
import { LucidePencil } from "../icons/LucidePencil";
import { LucideTrash2 } from "../icons/LucideTrash2";

const FaqSchema = v.object({
	question: v.string(),
	answer: v.string(),
	id: v.optional(v.pipe(v.string(), v.transform(Number), v.integer())),
});
type Faq = v.InferOutput<typeof FaqSchema>;

const parent = $id("faqs");

const headers = { RequestVerificationToken: parent.dataset.csrf ?? "" };

const FAQ = () => {
	const [faqs, { refetch }] = createResource(
		async () => {
			const res = await GetApiFaqs();
			return res.data;
		},
		{ initialValue: [] },
	);

	const [form, setForm] = createStore<Faq>({ question: "", answer: "" });

	const deleteFaq = async (id: number) => {
		const res = await DeleteApiFaqs(id, headers);
		if (res.ok) {
			refetch();
		} else {
			throw new Error(res.data ?? res.statusText);
		}
	};

	const createFaq = async (e: SubmitEvent) => {
		e.preventDefault();

		const [error, data] = getFormData(e, FaqSchema);

		if (error) {
			console.error(error);
			return;
		}

		if (data.id) {
			const res = await PutApiFaqs({ ...data, id: data.id }, headers);
			if (res.ok) {
				refetch();
			} else {
				throw new Error(res.data ?? res.statusText);
			}
		} else {
			const res = await PostApiFaqs(data, headers);
			if (res.ok) {
				refetch();
			} else {
				throw new Error(res.data ?? res.statusText);
			}
		}
		setForm({ question: "", answer: "", id: undefined });
	};

	const openForEdit = (faq: FaqDto) => {
		setForm(faq);
	};

	return (
		<>
			<form class="form" onSubmit={createFaq}>
				<div class="o-form-group">
					<label for="question">Question</label>
					<input class="o-form-control" name="question" prop:value={form.question} />
				</div>
				<div class="o-form-group">
					<label for="answer">Answer</label>
					<textarea class="o-form-control" name="answer" prop:value={form.answer} />
				</div>
				<div class="o-form-group">
					<input class="o-form-control btn" type="submit" value="Submit" />
				</div>
				<Show when={form.id}>
					<input type="hidden" name="id" prop:value={form.id} />
				</Show>
			</form>

			<br />

			<For each={faqs()}>
				{(faq) => (
					<details class="details">
						<summary>{faq.question}</summary>
						<div class="actions">
							<button type="button" class="small inline action-btn" onClick={[deleteFaq, faq.id]}>
								<LucideTrash2 />
							</button>
							<button type="button" class="small inline action-btn" onClick={[openForEdit, faq]}>
								<LucidePencil />
							</button>
						</div>
						<div innerHTML={faq.answerRendered} />
					</details>
				)}
			</For>
		</>
	);
};

render(() => <FAQ />, parent);
