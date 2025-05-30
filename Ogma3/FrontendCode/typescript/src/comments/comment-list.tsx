import { type Component, createEffect, createMemo, createResource, createSignal, For, onMount } from "solid-js";
import { GetApiComments } from "@g/paths-public";
import { CommentListPagination } from "./comment-list-pagination";
import type { CommentDto } from "@g/types-public";
import { Comment } from "./comment";

export interface CommentListFunctions {
	submitted: (sqid: string) => void;
}

interface Props {
	id: number;
	ref?: (functions: CommentListFunctions) => void;
}

export const CommentList: Component<Props> = (props) => {
	const [currentPage, setCurrentPage] = createSignal(1);
	const [highlight, setHighlight] = createSignal<string | null>(null);
	const [reload, setReload] = createSignal(0);

	const [commentsData] = createResource(
		() => [props.id, currentPage(), highlight(), reload()] as const,
		async ([id, page, _]) => {
			const res = await GetApiComments(id, page, highlight(), {
				// TODO: Remove after new comment system is done
				"X-Markdown": "true",
			});
			if (!res.ok) {
				console.error(highlight());
				throw new Error(res.error ?? res.statusText);
			}
			return res.data;
		},
	);

	let timeout: ReturnType<typeof setTimeout> | undefined;
	const changeHighlight = (idx: string | null) => {
		if (timeout) {
			clearTimeout(timeout);
		}

		timeout = setTimeout(() => {
			document.getElementById(`comment-${idx}`)?.scrollIntoView({
				behavior: "smooth",
				block: "center",
				inline: "nearest",
			});
			history.replaceState(undefined, "", `#comment-${idx}`);
		}, 10);
	};

	createEffect(() => {
		changeHighlight(highlight());
	});

	const comments = createMemo<CommentDto[]>((prev) => {
		const data = commentsData();

		if (commentsData.state === "ready" && data) {
			return data.elements;
		}

		return prev ?? [];
	});
	const totalComments = () => commentsData()?.total ?? 0;
	const commentsPerPage = () => commentsData()?.perPage ?? 0;
	const totalPages = () => commentsData()?.pages ?? 0;

	onMount(() => {
		props.ref?.({
			submitted: (sqid: string) => {
				console.log("Submitted");
				setCurrentPage(1);
				setHighlight(sqid);
			},
		});
	});

	const changePage = (page: number) => {
		if (page === currentPage()) {
			return;
		}
		setHighlight(null);
		setCurrentPage(page);
	};

	const pagination = () => (
		<CommentListPagination
			total={totalComments()}
			page={currentPage()}
			perPage={commentsPerPage()}
			changePage={changePage}
			prevPage={() => changePage(Math.max(1, currentPage() - 1))}
			nextPage={() => changePage(Math.min(totalPages(), currentPage() + 1))}
		/>
	);

	return (
		<div>
			{pagination()}
			<For each={comments()}>
				{(c) => (
					<Comment
						onDelete={() => setReload(reload() + 1)}
						onHighlight={setHighlight}
						marked={c.sqid === highlight()}
						{...c}
					/>
				)}
			</For>
			{pagination()}
		</div>
	);
};
