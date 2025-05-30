import type { CommentDto } from "@g/types-public";
import { long } from "@h/tinytime-templates";
import { DeleteApiComments } from "@g/paths-public";

type Props = CommentDto & {
	marked: boolean;
	onDelete: () => void;
	onHighlight: (id: string) => void;
};

export const Comment = (props: Props) => {
	const date = (dt: Date) => long.render(dt);

	const changeHighlight = (e: Event) => {};

	const report = (e: Event) => {};
	const del = async (e: Event) => {
		console.log(`Yeeting ${props.sqid}...`);
		const res = await DeleteApiComments(props.sqid);
		if (res.ok) {
			props.onDelete();
		} else {
			console.error(res.error);
		}
	};
	const edit = (e: Event) => {};

	return (
		<div id={`comment-${props.sqid}`} classList={{ comment: true, highlight: props.marked }}>
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
					<a class="link" href={`#comment-${props.sqid}`} onClick={[props.onHighlight, props.sqid]}>
						#{props.sqid}
					</a>

					<p class="sm-line" />

					<time datetime={new Date(props.dateTime).toISOString()} class="time">
						{date(new Date(props.dateTime))}
					</time>

					<div class="actions">
						<button type={"button"} class="action-btn small red-hl" title="Report" onClick={report}>
							<o-icon icon="lucide:flag" class="material-icons-outlined icon" />
						</button>
						<button type={"button"} class="action-btn small" title="Delete" onClick={del}>
							<o-icon icon="lucide:trash-2" class="material-icons-outlined icon" />
						</button>
						<button type={"button"} class="action-btn small" title="Edit" onClick={edit}>
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
