export const HiddenCommentBody = (props: { onToggleVisibility: () => void }) => {
	return (
		<div class="main" onclick={props.onToggleVisibility}>
			<div class="header">Comment hidden by user blacklist</div>
		</div>
	);
};
