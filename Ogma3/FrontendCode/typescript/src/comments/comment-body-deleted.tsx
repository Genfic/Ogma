import { long } from "@h/tinytime-templates";

export const DeletedCommentBody = (props: { creationDate: Date; deletedBy: string | null }) => {
	const date = (dt: Date) => long.render(dt);

	return (
		<div class="main">
			<div class="header">
				<time datetime={props.creationDate.toISOString()} class="time">
					{date(props.creationDate)}
				</time>
				<p class="sm-line" />
				<span>Comment deleted by {props.deletedBy?.toLowerCase()}.</span>
			</div>
		</div>
	);
};
