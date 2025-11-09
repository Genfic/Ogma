import { GetApiCommentsThread, PostApiComments, PostApiCommentsThreadLock } from "@g/paths-public";
import type { CommentSource } from "@g/types-public";
import { component } from "@h/web-components";
import { noShadowDOM } from "solid-element";
import { createResource, createSignal, Show } from "solid-js";
import { CommentList, type CommentListFunctions } from "./comments/comment-list";
import css from "./comments.css";
import { LucideCircleHelp } from "./icons/LucideCircleHelp";
import { LucideMessageSquarePlus } from "./icons/LucideMessageSquarePlus";
import { MdiLockOpenVariantOutline } from "./icons/MdiLockOpenVariantOutline";
import { MdiLockOutline } from "./icons/MdiLockOutline";

interface Props {
	csrf: string;
	threadId: number;
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
			const res = await GetApiCommentsThread(props.threadId);
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
			await send();
		}
	};

	const submit = async (e: Event) => {
		e.preventDefault();
		await send();
	};

	const send = async () => {
		if (body().trim().length >= maxLength()) {
			return;
		}

		const res = await PostApiComments(
			{
				body: body(),
				thread: Number(props.threadId),
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
		const res = await PostApiCommentsThreadLock({ threadId: props.threadId });
		if (!res.ok) {
			console.error(res.error);
		}
		setIsLocked(res.ok && res.data);
	};

	return (
		<div class="comments-container" style={{ width: "100%", opacity: 1, margin: "auto" }}>
			<Show when={!props.isLoggedIn}>
				<div class="info">
					<a href={props.loginRoute}>Log in</a>&nbsp;or&nbsp;
					<a href={props.registerRoute}>register</a> to be able to leave comments
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
							onInput={(e) => setBody(e.target.value)}
							value={body()}
							onKeyUp={handleEnterKey}
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
							<button type="submit" class="comment-btn active-border" onClick={submit}>
								<LucideMessageSquarePlus />
								Comment
							</button>
							<a class="help-btn active-border" rel="noreferrer" target="_blank" href={props.mdRefRoute}>
								<LucideCircleHelp />
							</a>
						</div>
					</form>
				</Show>
				<Show when={threadData.loading}>Loading...</Show>
				<Show when={threadData.error}>Error: {threadData.error}</Show>

				<div class="buttons">
					<Show when={isStaff()}>
						<button type="button" class="action-btn" classList={{ active: isLocked() }} onClick={lock}>
							{isLocked() ? <MdiLockOutline /> : <MdiLockOpenVariantOutline />}
							&nbsp;
							{isLocked() ? "Unlock" : "Lock"}
						</button>
					</Show>
				</div>
			</Show>

			<CommentList ref={(f) => (listRef = f)} id={props.threadId} />
		</div>
	);
};

component(
	"o-comments",
	{
		csrf: "",
		threadId: 0,
		isLoggedIn: false,
		lockDate: null,
		mdRefRoute: "",
		loginRoute: "",
		registerRoute: "",
	},
	Comments,
	css,
);
