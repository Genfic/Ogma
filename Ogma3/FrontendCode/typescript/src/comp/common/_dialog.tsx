import { createUniqueId, type JSX, onMount, type ParentComponent, type ParentProps, Show } from "solid-js";
import { createEventDispatcher } from "@solid-primitives/event-dispatcher";

export type DialogApi = {
	open: () => void;
	close: () => void;
};

type Props = {
	classes?: string[];
	contentClass?: string;
	header?: JSX.Element;
	ref?: (api: DialogApi) => void;
	onClose?: (evt: CustomEvent) => void;
	onOpen?: (evt: CustomEvent) => void;
};

export const Dialog: ParentComponent<Props> = (props: ParentProps<Props>) => {
	let maybeDialogRef: HTMLDialogElement | undefined;
	let dialogRef: HTMLDialogElement;

	const id = createUniqueId();

	const dispatch = createEventDispatcher(props);

	const open = () => {
		dialogRef.showModal();
		dispatch("open");
	};

	const close = () => {
		dialogRef.close();
		dispatch("close");
	};

	onMount(() => {
		if (!maybeDialogRef) {
			throw new Error("Dialog not mounted");
		}

		dialogRef = maybeDialogRef;

		props.ref?.({
			open,
			close,
		});
	});

	const backdropClose = (e: MouseEvent & { currentTarget: HTMLDialogElement }) => {
		const rect = dialogRef.getBoundingClientRect();
		const minX = rect.left + dialogRef.clientLeft;
		const minY = rect.top + dialogRef.clientTop;
		if (
			e.clientX < minX ||
			e.clientX >= minX + dialogRef.clientWidth ||
			e.clientY < minY ||
			e.clientY >= minY + dialogRef.clientHeight
		) {
			close();
		}
	};

	return (
		<dialog
			id={id}
			ref={(e) => (maybeDialogRef = e)}
			class={["my-dialog", ...(props.classes ?? [])].join(" ")}
			onmousedown={backdropClose}
			aria-modal={true}
		>
			<button type="button" aria-controls={id} class="close-btn" autofocus onClick={close}>
				<o-icon icon="lucide:x" />
			</button>

			<Show when={props.header}>
				<div class="header">{props.header}</div>
			</Show>

			<div class={`content ${props.contentClass}`}>{props.children}</div>
		</dialog>
	);
};
