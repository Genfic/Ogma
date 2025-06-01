import { DeleteApiComments } from "@g/paths-public";
import type { CommentDto } from "@g/types-public";
import { long } from "@h/tinytime-templates";
import { createSignal, Match, Show, Switch } from "solid-js";
import { DeletedCommentBody } from "./comment-body-deleted";
import { HiddenCommentBody } from "./comment-body-hidden";

type Props = CommentDto & {
	key: number;
	marked: boolean;
	owner: string | null;
	onDelete: () => void;
	onHighlightChange: (idx: number) => void;
};
const date = (dt: Date) => long.render(dt);

export const Comment = (props: Props) => {
	const [hidden, setHidden] = createSignal(props.isBlocked);

	const report = () => {};
	const del = async () => {
		const res = await DeleteApiComments(props.id);
		if (res.ok) {
			props.onDelete();
		} else {
			console.error(res.error);
		}
	};
	const edit = () => {};

	const userOwnsComment = () =>
		props.owner && props.author && props.owner.toLowerCase() === props.author.userName.toLowerCase();

	return (
		<div id={`comment-${props.id}`} classList={{ comment: true, highlight: props.marked }}>
			<Switch>
				<Match when={hidden()}>
					<HiddenCommentBody onToggleVisibility={() => setHidden(!hidden())} />
				</Match>
				<Match when={props.deletedBy}>
					<DeletedCommentBody creationDate={new Date(props.dateTime)} deletedBy={props.deletedBy} />
				</Match>
				<Match when={!hidden() && !props.deletedBy}>
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
								href={`#comment-${props.key + 1}`}
								onClick={[props.onHighlightChange, props.key + 1]}
							>
								#{props.key + 1}
							</a>

							<p class="sm-line" />

							<time datetime={new Date(props.dateTime).toISOString()} class="time">
								{date(new Date(props.dateTime))}
							</time>

							<div class="actions">
								<Show when={props.owner && !userOwnsComment()}>
									<button
										type={"button"}
										class="action-btn small red-hl"
										title="Report"
										onclick={report}
									>
										<o-icon icon="lucide:flag" class="material-icons-outlined icon" />
									</button>
								</Show>
								<Show when={userOwnsComment()}>
									<button type={"button"} class="action-btn small" title="Delete" onclick={del}>
										<o-icon icon="lucide:trash-2" class="material-icons-outlined icon" />
									</button>
									<button type={"button"} class="action-btn small" title="Edit" onclick={edit}>
										<o-icon icon="lucide:pencil" class="material-icons-outlined icon" />
									</button>
								</Show>
							</div>
						</div>

						{props.body && <div class="body md" innerHTML={props.body} />}
					</div>
				</Match>
			</Switch>
		</div>
	);
};
