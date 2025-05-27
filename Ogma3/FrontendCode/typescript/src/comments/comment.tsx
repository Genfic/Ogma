import type { CommentDto } from "@g/types-public";
import { long } from "@h/tinytime-templates";

type Props = CommentDto & { key: number; marked: boolean };

export const Comment = (props: Props) => {
	const date = (dt: Date) => long.render(dt);

	const changeHighlight = (e: Event) => {};

	const report = (e: Event) => {};
	const del = (e: Event) => {};
	const edit = (e: Event) => {};

	return (
		<div id={`comment-${props.id}`} classList={{ comment: true, highlight: props.marked }}>
			{props.author && (
				<div class="author">
					<a href={`/user/${props.author.userName}`} class="name">
						{props.author.userName}
					</a>

					{props.author.roles[0] && (
						<div class="role-tag">
							<span class="name">{props.author.roles[0].name}</span>
							<div class="bg" style={{ "background-color": props.author.roles[0].color ?? "white" }} />
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
					<a class="link" href={`#comment-${props.key + 1}`} onClick={changeHighlight}>
						#{props.key + 1}
					</a>

					<p class="sm-line" />

					<time datetime={new Date(props.dateTime).toISOString()} class="time">
						{date(new Date(props.dateTime))}
					</time>

					<div class="actions">
						<button type={"button"} class="action-btn small red-hl" title="Report" onclick={report}>
							<o-icon icon="lucide:flag" class="material-icons-outlined icon" />
						</button>
						<button type={"button"} class="action-btn small" title="Delete" onclick={del}>
							<o-icon icon="lucide:trash-2" class="material-icons-outlined icon" />
						</button>
						<button type={"button"} class="action-btn small" title="Edit" onclick={edit}>
							<o-icon icon="lucide:pencil" class="material-icons-outlined icon" />
						</button>
					</div>
				</div>

				{props.body && (
					<div
						// v-if="props.body && (!editData || editData.id !== props.id)"
						class="body md"
						innerHTML={props.body ?? ""}
					/>
				)}
			</div>
		</div>
	);
};
