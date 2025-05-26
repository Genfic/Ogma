import { customElement, noShadowDOM } from "solid-element";
import { createResource, createSignal, Show } from "solid-js";
import { GetApiCommentsThread, PostApiComments, PostApiCommentsThreadLock } from "@g/paths-public";
import type { CommentSource } from "@g/types-public";
import { CommentList, type CommentListFunctions } from "./comments/comment-list";
import { Styled } from "./comp/common/_styled";
import css from "./comments.css";

interface Props extends Record<string, unknown> {
	csrf: string;
	id: number;
	isLoggedIn: boolean;
	lockDate: Date | null;
	mdRefRoute: string;
	loginRoute: string;
	registerRoute: string;
}

const Comments = (props: Props) => {
	noShadowDOM();

	let listRef: CommentListFunctions | undefined;

	const [body, setBody] = createSignal("");
	const [threadData] = createResource(
		async () => {
			const res = await GetApiCommentsThread(props.id);
			if (!res.ok) {
				throw new Error(res.error ?? res.statusText);
			}
			const isStaff = res.headers.get("X-IsStaff")?.toLowerCase() === "true";
			return { ...res.data, isStaff };
		},
		{
			initialValue: {
				maxCommentLength: 0,
				minCommentLength: 0,
				perPage: 10,
				isStaff: false,
				isLocked: false,
				source: "" as CommentSource,
			},
		},
	);

	const [isLocked, setIsLocked] = createSignal(props.lockDate !== null);

	const maxLength = () => threadData().maxCommentLength;
	const isStaff = () => threadData().isStaff;

	const handleEnterKey = async (e: KeyboardEvent) => {
		if (e.key === "Enter" && e.ctrlKey) {
			e.preventDefault();
			await submit(e);
		}
	};

	const submit = async (e: Event) => {
		e.preventDefault();

		if (body().length >= maxLength()) {
			return;
		}

		const res = await PostApiComments(
			{
				body: body(),
				thread: props.id,
				source: threadData()?.source ?? ("" as CommentSource),
			},
			{ RequestVerificationToken: props.csrf },
		);

		if (!res.ok) {
			return;
		}

		setBody("");
		listRef?.submitted();
	};

	const lock = async () => {
		if (!isStaff()) {
			return false;
		}
		const res = await PostApiCommentsThreadLock({ threadId: props.id });
		if (!res.ok) {
			console.error(res.error);
		}
		setIsLocked(res.ok && res.data);
	};

	return (
		<div class="comments-container" style={{ width: "100%", opacity: 1, margin: "auto" }}>
			<Show when={!props.isLoggedIn}>
				<div class="info">
					<a href={props.loginRoute}>Log in</a> or
					<a href={props.registerRoute}>register</a>
					to be able to leave comments
				</div>
			</Show>

			<Show when={props.isLoggedIn}>
				<Show when={isLocked()}>
					<div class="info">Thread is locked.</div>
				</Show>
				<Show when={!isLocked() && threadData()}>
					<form>
						<textarea
							class="comment-box active-border"
							onChange={(e) => setBody(e.target.value)}
							value={body()}
							onKeyDown={handleEnterKey}
							name="body"
							id="body"
							rows="3"
							aria-label="Comment"
						/>

						<div class="counter" classList={{ invalid: body().length >= maxLength() }}>
							<div
								class="o-progress-bar"
								style={{ width: `${Math.min(100, 100 * (body().length / maxLength()))}%` }}
							/>
							<span>
								{body().length} / {maxLength()}
							</span>
						</div>

						<div class="buttons">
							<button type="submit" class="comment-btn active-border" onclick={submit}>
								<o-icon icon="lucide:message-square-plus" />
								Comment
							</button>
							<a class="help-btn active-border" rel="noreferrer" target="_blank" href={props.mdRefRoute}>
								<o-icon icon="lucide:circle-help" />
							</a>
						</div>
					</form>
				</Show>
				<Show when={threadData.loading}>Loading...</Show>
				<Show when={threadData.error}>Error: {threadData.error}</Show>

				<div class="buttons">
					<Show when={isStaff()}>
						<button type="button" class="action-btn" classList={{ active: isLocked() }} onClick={lock}>
							<o-icon icon={isLocked() ? "mdi:lock-outline" : "mdi:lock-open-variant-outline"} />
							&nbsp;
							{isLocked() ? "Unlock" : "Lock"}
						</button>
					</Show>
				</div>
			</Show>

			<CommentList ref={(f) => (listRef = f)} id={props.id} />
		</div>
	);
};

customElement(
	"o-comments",
	{
		csrf: "",
		id: 0,
		isLoggedIn: false,
		lockDate: null,
		mdRefRoute: "",
		loginRoute: "",
		registerRoute: "",
	},
	Styled(Comments, css),
);
