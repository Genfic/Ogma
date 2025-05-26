import { For, Show } from "solid-js";

interface Props {
	page: number;
	total: number;
	perPage: number;
	nextPage: () => void;
	prevPage: () => void;
	changePage: (page: number) => void;
}

export const CommentListPagination = (props: Props) => {
	const pages = () => {
		return Array.from({ length: Math.ceil(props.total / props.perPage) }, (_, i) => i + 1).filter(
			(idx) => idx >= props.page - 4 && idx <= Math.max(props.page, 5),
		);
	};

	return (
		<Show when={props.total > props.perPage}>
			<div class="pagination">
				<button type="button" class="prev button" onClick={props.prevPage}>
					Previous
				</button>
				<Show when={props.page > 5}>
					<button type="button" class="ph button">
						...
					</button>
				</Show>
				<For each={pages()}>
					{(idx) => (
						<button
							type="button"
							onClick={[props.changePage, idx]}
							classList={{ active: idx === props.page }}
							class="page button"
						>
							{idx}
						</button>
					)}
				</For>
				<Show when={props.total / props.perPage > 5 && props.page !== props.total / props.perPage}>
					<button type="button" class="ph button">
						...
					</button>
				</Show>
				<button type="button" class="next button" onClick={props.nextPage}>
					Next
				</button>
			</div>
		</Show>
	);
};
