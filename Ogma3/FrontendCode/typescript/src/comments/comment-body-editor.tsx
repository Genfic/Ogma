import { GetApiCommentsMd, PatchApiComments } from "@g/paths-public";
import { createResource, createSignal } from "solid-js";
import { LucidePencil } from "../icons/LucidePencil";
import { LucideX } from "../icons/LucideX";

type Props = {
	id: number;
	onCancel: () => void;
	onUpdate: (body: string) => void;
};

export const CommentBodyEditor = (props: Props) => {
	const [text, setText] = createSignal("");

	const [body] = createResource(async () => {
		const res = await GetApiCommentsMd(props.id);
		if (res.ok) {
			return res.data;
		}
		throw new Error(res.error);
	});

	const enter = async (e: KeyboardEvent) => {
		if (e.key === "Enter" && e.ctrlKey) {
			await update(e);
		}
	};

	const submit = async (e: Event) => {
		e.preventDefault();
		await update(e);
	};

	const update = async (_e: Event) => {
		if (text().trim().length < 1) return;

		const res = await PatchApiComments({
			body: text(),
			commentId: props.id,
		});
		if (res.ok) {
			setText("");
			props.onUpdate(res.data.body);
		} else {
			console.error(res.error);
		}
	};

	return (
		<div class="main">
			<form class="form">
				<textarea
					class="comment-box"
					onInput={(e) => setText(e.target.value)}
					onKeyUp={enter}
					name="body"
					id="edit-body"
					rows="3"
					aria-label="Comment"
					value={body()}
				/>

				<div class="buttons">
					<button type="submit" class="confirm active-border" onClick={submit}>
						<LucidePencil />
						Update
					</button>
					<button type="reset" class="cancel active-border" onClick={() => props.onCancel()}>
						<LucideX />
						Cancel
					</button>
				</div>
			</form>
		</div>
	);
};
