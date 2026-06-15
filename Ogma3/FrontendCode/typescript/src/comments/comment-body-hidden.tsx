export const HiddenCommentBody = (props: { onToggleVisibility: () => void }) => {
	const handleKey = (e: KeyboardEvent) => {
		if (e.key === "Enter" || e.key === " ") {
			props.onToggleVisibility();
		}
	};

	return (
		<button type="button" class="main" onclick={props.onToggleVisibility} onKeyDown={handleKey}>
			<div class="header">Comment hidden by user blacklist</div>
		</button>
	);
};
