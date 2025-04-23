import { createUniqueId, onMount, type ParentComponent } from "solid-js";

export type DialogApi = {
	open: () => void;
};

export type DialogType = ParentComponent<{ classes?: string; ref?: (api: DialogApi) => void }>;

export const Dialog: DialogType = (props) => {
	let dialogRef: HTMLDialogElement | undefined;
	const id = createUniqueId();

	const open = () => dialogRef?.showModal();

	onMount(() => {
		props.ref?.({
			open,
		});
	});

	const backdropClose = (e: MouseEvent & { currentTarget: HTMLDialogElement }) => {
		const rect = dialogRef?.getBoundingClientRect();
		const minX = rect.left + dialogRef?.clientLeft;
		const minY = rect.top + dialogRef?.clientTop;
		if (
			e.clientX < minX ||
			e.clientX >= minX + dialogRef?.clientWidth ||
			e.clientY < minY ||
			e.clientY >= minY + dialogRef?.clientHeight
		) {
			dialogRef?.close();
		}
	};

	return (
		<dialog
			id={id}
			ref={dialogRef}
			class={`my-dialog ${props.classes}`}
			onmousedown={backdropClose}
			aria-modal={true}
		>
			<button type="button" aria-controls={id} class="close-btn" autofocus onClick={() => dialogRef?.close()}>
				<o-icon icon="lucide:x" />
			</button>

			<div class="content">{props.children}</div>
		</dialog>
	);
};
