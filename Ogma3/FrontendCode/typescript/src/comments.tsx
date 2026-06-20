import LucideCircleHelp from "icon:lucide:circle-question-mark";
import LucideMessageSquarePlus from "icon:lucide:message-square-plus";
import MdiLockOpenVariantOutline from "icon:mdi:lock-open-variant-outline";
import MdiLockOutline from "icon:mdi:lock-outline";
import { Comment } from "@g/ctconfig";
import { GetApiCommentsThread, GetApiPowIssue, PostApiComments, PostApiCommentsThreadLock } from "@g/paths-public";
import type { CommentSource } from "@g/types-public";
import { minePow, type PowResult } from "@h/pow";
import { component } from "@h/web-components";
import { createVisibilityObserver } from "@solid-primitives/intersection-observer";
import { noShadowDOM } from "solid-element";
import { createEffect, createResource, Show } from "solid-js";
import { CommentList, type CommentListFunctions } from "./comments/comment-list";
import css from "./comments.css";

interface Props {
	csrf: string;
	threadId: number;
	isLoggedIn: boolean;
	lockDate?: Date;
	mdRefRoute: string;
	loginRoute: string;
	registerRoute: string;
}

const adjectives = [
	"insightful",
	"thoughtful",
	"engaging",
	"heartfelt",
	"inspiring",
	"uplifting",
	"perceptive",
	"encouraging",
	"warm",
	"witty",
	"clever",
	"eloquent",
	"sincere",
	"appreciative",
	"supportive",
	"charming",
	"reflective",
	"delightful",
	"constructive",
	"nuanced",
	"expressive",
	"resonant",
	"poetic",
	"admiring",
	"genuine",
];

const article = (adjective: string) => (["a", "e", "i", "o", "u"].includes(adjective[0].toLowerCase()) ? "an" : "a");

const adj = adjectives[Math.floor(Math.random() * adjectives.length)];
const pre = article(adj);

const maxCommentLength = Comment.MaxBodyLength;
// const minCommentLength = Comment.MinBodyLength;

const Comments = (props: Props) => {
	noShadowDOM();

	let listRef: CommentListFunctions | undefined;

	let body = $signal("");
	let powResult = $signal<PowResult & { expiry: Date; token: string }>();

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
				perPage: 10,
				isStaff: false,
				isLocked: false,
				source: "" as CommentSource,
			},
		},
	);

	const getPow = async () => {
		const res = await GetApiPowIssue();
		if (!res.ok) {
			throw new Error(res.data ?? res.statusText);
		}
		return res.data;
	};

	let isLocked = $signal(!!props.lockDate);

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
		const pow = await getPow();
		const result = await minePow(pow.token, pow.difficulty);

		console.log("Pow result", pow, result);
		powResult = { ...result, expiry: pow.expiresAt, token: pow.token };
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

	let mining = $signal(false);
	const send = async () => {
		if (body.trim().length >= maxCommentLength) {
			return;
		}

		if (!powResult || powResult.expiry < new Date()) {
			console.log(`Pow ${powResult ? "expired" : "undefined"}, fetching new one`);
			const pow = await getPow();
			mining = true;
			const result = await minePow(pow.token, pow.difficulty);
			powResult = { ...result, expiry: pow.expiresAt, token: pow.token };
		}

		const res = await PostApiComments(
			{
				body: body,
				thread: Number(props.threadId),
				source: threadData()?.source ?? ("" as CommentSource),
				powToken: powResult.token,
				powNonce: powResult.nonce,
				powHash: powResult.hash,
			},
			{ RequestVerificationToken: props.csrf },
		);

		powResult = undefined;
		mining = false;

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
							placeholder={`Write ${pre} ${adj} comment...`}
						/>

						<div class="counter" classList={{ invalid: body.length >= maxCommentLength }}>
							<div
								class="o-progress-bar"
								style={{ width: `${Math.min(100, 100 * (body.length / maxCommentLength))}%` }}
							/>
							<span>
								{body.length} / {maxCommentLength}
							</span>
						</div>

						<div class="buttons">
							<button
								type="submit"
								classList={{ "comment-btn": true, "active-border": true, loading: mining }}
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
	},
	Comments,
	css,
	["lockDate"],
);
