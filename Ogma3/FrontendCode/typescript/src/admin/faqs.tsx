import { DeleteApiFaqs, GetApiFaqs, PostApiFaqs, PutApiFaqs } from "@g/paths-public";
import type { FaqDto } from "@g/types-public";
import { $id } from "@h/dom";
import { createResource, For } from "solid-js";
import { render } from "solid-js/web";

interface Faq {
	question: string;
	answer: string;
	id?: number | undefined;
}

const parent = $id("faqs");

const headers = { RequestVerificationToken: parent.dataset.csrf ?? "" };

const FAQ = () => {
	const [faqs, { refetch }] = createResource(
		async () => {
			const res = await GetApiFaqs();
			if (!res.ok) {
				throw new Error(res.error);
			}
			return res.data;
		},
		{ initialValue: [] },
	);

	let formData = $signal<Faq>({ question: "", answer: "" });

	const handleInput = (e: InputEvent) => {
		const target = e.target as HTMLInputElement;
		console.log(`Setting ${target.name} to ${target.value}`);
		formData = {
			...formData,
			[target.name]: target.value,
		};
	};

	const deleteFaq = async (id: number) => {
		const res = await DeleteApiFaqs(id, headers);
		if (res.ok) {
			refetch();
		} else {
			throw new Error(res.error);
		}
	};

	const createFaq = async (e: SubmitEvent) => {
		e.preventDefault();

		const data = formData;
		if (!data) return;

		if (data.id) {
			const res = await PutApiFaqs({ id: data.id, question: data.question, answer: data.answer }, headers);
			if (res.ok) {
				refetch();
			} else {
				throw new Error(res.error);
			}
		} else {
			const res = await PostApiFaqs({ question: data.question, answer: data.answer }, headers);
			if (res.ok) {
				refetch();
			} else {
				throw new Error(res.error);
			}
		}
		clear();
	};

	const openForEdit = (faq: FaqDto) => {
		formData = faq;
	};

	const clear = () => {
		formData = { question: "", answer: "", id: undefined };
	};

	return (
		<>
			<form class="form" onSubmit={createFaq}>
				<div class="o-form-group">
					<label for="question">Question</label>
					<input class="o-form-control" name="question" value={formData?.question} onInput={handleInput} />
				</div>
				<div class="o-form-group">
					<label for="answer">Answer</label>
					<textarea class="o-form-control" name="answer" value={formData?.answer} onInput={handleInput} />
				</div>
				<div class="o-form-group">
					<input class="o-form-control btn" type="submit" value="Submit" />
				</div>
				{(() => {
					if (formData?.id) {
						return <input type="hidden" name="id" value={formData.id} />;
					}
					return null;
				})()}
			</form>

			<For each={faqs()}>
				{(faq) => (
					<details class="details">
						<summary>{faq.question}</summary>
						<div class="actions">
							<button type="button" class="small inline action-btn" onClick={[deleteFaq, faq.id]}>
								<o-icon icon="lucide:trash-2" />
							</button>
							<button type="button" class="small inline action-btn" onClick={[openForEdit, faq]}>
								<o-icon icon="lucide:pencil" />
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
