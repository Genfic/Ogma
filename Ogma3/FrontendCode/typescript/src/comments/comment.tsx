import { DeleteApiComments, GetApiCommentsRevisions } from "@g/paths-public";
import type { CommentDto, GetRevisionResult } from "@g/types-public";
import { toCurrentTimezone } from "@h/date-helpers";
import { long } from "@h/tinytime-templates";
import { createSignal, For, Match, Show, Switch } from "solid-js";
import { Markdown } from "../comp/common/_markdown";
import type { ReportModalElement } from "../comp/report-modal";
import { LucideFlag } from "../icons/LucideFlag";
import { LucideLink } from "../icons/LucideLink";
import { LucidePencil } from "../icons/LucidePencil";
import { LucideTrash2 } from "../icons/LucideTrash2";
import { DeletedCommentBody } from "./comment-body-deleted";
import { CommentBodyEditor } from "./comment-body-editor";
import { HiddenCommentBody } from "./comment-body-hidden";

type Props = CommentDto & CommentProps;

export type CommentProps = {
	marked: boolean;
	owner: string | null;
	onDelete: () => void;
	onHighlightChange: (e: MouseEvent, idx: string) => void;
};

const date = (dt: Date) => long.render(toCurrentTimezone(dt));

export const Comment = (props: Props) => {
	const [hidden, setHidden] = createSignal(props.isBlocked);
	const [body, setBody] = createSignal(props.body);
	const [editing, setEditing] = createSignal(false);
	const [revisions, setRevisions] = createSignal<GetRevisionResult[]>([]);
	const [showRevision, setShowRevision] = createSignal(false);

	const report = () => {
		const modal = document.getElementById("report-comment") as ReportModalElement;
		if (!modal) return;
		modal.createNew(props.id, "Comment");
	};
	const del = async () => {
		if (!confirm("Are you sure you want to delete this comment?")) return;

		const res = await DeleteApiComments(props.id);
		if (res.ok) {
			props.onDelete();
		} else {
			console.error(res.error);
		}
	};
	const edit = () => {
		setEditing(true);
	};

	const updated = (body: string) => {
		setEditing(false);
		setBody(body);
	};

	const history = async () => {
		if (showRevision()) {
			setShowRevision(false);
			return;
		}

		const res = await GetApiCommentsRevisions(props.id);
		if (res.ok) {
			setRevisions(res.data);
			setShowRevision(true);
		} else {
			console.error(res.error);
		}
	};

	const commentState = () => {
		if (hidden()) {
			return "hidden";
		}
		if (props.deletedBy) {
			return "deleted";
		}
		if (editing()) {
			return "editing";
		}
		return "regular";
	};

	const userOwnsComment = () =>
		props.owner && props.author && props.owner.toLowerCase() === props.author.userName.toLowerCase();

	return (
		<div id={`comment-${props.id}`} classList={{ comment: true, marked: props.marked }}>
			<Switch>
				<Match when={commentState() === "hidden"}>
					<HiddenCommentBody onToggleVisibility={() => setHidden(!hidden())} />
				</Match>
				<Match when={commentState() === "deleted"}>
					<DeletedCommentBody creationDate={new Date(props.dateTime)} deletedBy={props.deletedBy} />
				</Match>
				<Match when={commentState() === "editing"}>
					<CommentBodyEditor
						id={props.id}
						body={props.body}
						onCancel={() => setEditing(false)}
						onUpdate={updated}
					/>
				</Match>
				<Match when={commentState() === "regular"}>
					{props.author && (
						<div class="author">
							<a href={`/user/${props.author.userName}`} class="name">
								{props.author.userName}
							</a>

							{props.author.roles[0] && (
								<div class="role-tag">
									<span class="name">{props.author.roles[0].name}</span>
									<div
										class="bg"
										style={{
											"background-color": props.author.roles[0].color ?? "white",
										}}
									/>
								</div>
							)}

							<img
								src={props.author.avatar}
								alt={`${props.author.userName}'s avatar`}
								class="avatar"
								loading="lazy"
							/>
						</div>
					)}
					<div class="main">
						<div class="header">
							<a
								class="link"
								href={`#comment-${props.id}`}
								onClick={(e) => props.onHighlightChange(e, props.id)}
								title="Permalink"
							>
								<LucideLink />
							</a>

							<time datetime={props.dateTime.toISOString()} class="time">
								{date(props.dateTime)}
							</time>

							<div class="actions">
								<Show when={props.owner /*&& !userOwnsComment()*/}>
									<button
										type={"button"}
										class="action-btn small red-hl"
										title="Report"
										onclick={report}
									>
										<LucideFlag />
									</button>
								</Show>
								<Show when={userOwnsComment()}>
									<button type={"button"} class="action-btn small" title="Delete" onclick={del}>
										<LucideTrash2 />
									</button>
									<button type={"button"} class="action-btn small" title="Edit" onclick={edit}>
										<LucidePencil />
									</button>
								</Show>
							</div>
						</div>

						<Show when={body()} keyed>
							{(b) => <Markdown class={"body"} text={b} />}
						</Show>

						<Show when={props.isEdited}>
							<button type="button" onClick={history} class="edit-data">
								Edited
							</button>
						</Show>

						<Show when={showRevision()}>
							<div class="history">
								<For each={revisions()} fallback={<span>No revisions found</span>}>
									{(rev) => (
										<div class="item">
											<time class="time" datetime={rev.editTime.toISOString()}>
												{date(rev.editTime)}
											</time>
											<hr />
											<Markdown text={rev.body} class={"body"} />
										</div>
									)}
								</For>
							</div>
						</Show>
					</div>
				</Match>
			</Switch>
		</div>
	);
};
