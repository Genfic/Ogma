import { GetApiCommentsThread, PostApiComments, PostApiCommentsThreadLock } from "@g/paths-public";
import type { CommentSource } from "@g/types-public";
import { pow } from "@h/pow";
import { component } from "@h/web-components";
import { createVisibilityObserver } from "@solid-primitives/intersection-observer";
import { noShadowDOM } from "solid-element";
import { createEffect, createResource, Show } from "solid-js";
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
	lockDate?: Date;
	mdRefRoute: string;
	loginRoute: string;
	registerRoute: string;
	powToken: string;
	powDifficulty: number;
}

const Comments = (props: Props) => {
	noShadowDOM();

	let listRef: CommentListFunctions | undefined;

	let body = $signal("");
	let powResult = $signal<Awaited<ReturnType<typeof pow>>>();

	const [threadData] = createResource(
		async () => {
			const res = await GetApiCommentsThread(props.threadId);
			if (!res.ok) {
				throw new Error(res.data ?? res.statusText);
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

	let isLocked = $signal(!!props.lockDate);

	const maxLength = $memo(threadData().maxCommentLength);
	const isStaff = $memo(threadData().isStaff);

	const visibilityTrigger = $signal<Element>();
	const visible = createVisibilityObserver({ threshold: 0.5 })($get(visibilityTrigger));
	let listVisible = $signal(false);
	createEffect(() => {
		if (visible()) {
			listVisible = true;
		}
	});

	const textareaFocused = async () => {
		if (powResult) {
			return;
		}
		const result = await pow(props.powToken, props.powDifficulty);
		console.log("Pow result", result);
		powResult = result;
	};

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
		if (body.trim().length >= maxLength) {
			return;
		}

		if (!powResult) {
			return;
		}

		const res = await PostApiComments(
			{
				body: body,
				thread: Number(props.threadId),
				source: threadData()?.source ?? ("" as CommentSource),
				pow: {
					hash: powResult.hash,
					nonce: powResult.nonce,
					token: props.powToken,
				},
			},
			{ RequestVerificationToken: props.csrf },
		);

		if (!res.ok) {
			return;
		}

		body = "";
		listRef?.submitted(res.data);
	};

	const lock = async () => {
		if (!isStaff) {
			return false;
		}
		const res = await PostApiCommentsThreadLock({ threadId: props.threadId });
		if (!res.ok) {
			console.error(res.data ?? res.statusText);
		}
		isLocked = res.ok && res.data;
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
				<Show when={isLocked}>
					<div class="info">Thread is locked.</div>
				</Show>
				<Show when={!isLocked && threadData()}>
					<form>
						<textarea
							class="comment-box active-border"
							onInput={(e) => (body = e.target.value)}
							onFocus={textareaFocused}
							value={body}
							onKeyUp={handleEnterKey}
							name="body"
							id="body"
							rows="3"
							aria-label="Comment"
						/>

						<div class="counter" classList={{ invalid: body.length >= maxLength }}>
							<div
								class="o-progress-bar"
								style={{ width: `${Math.min(100, 100 * (body.length / maxLength))}%` }}
							/>
							<span>
								{body.length} / {maxLength}
							</span>
						</div>

						<div class="buttons">
							<button
								type="submit"
								class="comment-btn active-border"
								onClick={submit}
								disabled={powResult === undefined}
							>
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
					<Show when={isStaff}>
						<button type="button" class="action-btn" classList={{ active: isLocked }} onClick={lock}>
							{isLocked ? <MdiLockOutline /> : <MdiLockOpenVariantOutline />}
							&nbsp;
							{isLocked ? "Unlock" : "Lock"}
						</button>
					</Show>
				</div>
			</Show>

			<div ref={$set(visibilityTrigger)}>
				<Show when={listVisible} fallback={<p>Loading comments...</p>}>
					<CommentList ref={(f) => (listRef = f)} id={props.threadId} />
				</Show>
			</div>
		</div>
	);
};

component(
	"o-comments",
	{
		csrf: "",
		threadId: 0,
		isLoggedIn: false,
		lockDate: undefined,
		mdRefRoute: "",
		loginRoute: "",
		registerRoute: "",
		powToken: "",
		powDifficulty: 0,
	},
	Comments,
	css,
	["lockDate"],
);
