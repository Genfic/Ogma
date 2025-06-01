import { GetApiComments } from "@g/paths-public";
import type { CommentDto } from "@g/types-public";
import { stripNullish } from "@h/csharping";
import { type Component, createEffect, createMemo, createResource, createSignal, For, onMount } from "solid-js";
import { Comment } from "./comment";
import { CommentListPagination } from "./comment-list-pagination";

export interface CommentListFunctions {
	submitted: () => void;
}

interface Props {
	id: number;
	ref?: (functions: CommentListFunctions) => void;
}

export const CommentList: Component<Props> = (props) => {
	const [currentPage, setCurrentPage] = createSignal(1);
	const [highlight, setHighlight] = createSignal(0);
	const [reload, setReload] = createSignal(0);
	const [username, setUsername] = createSignal<string | null>(null);

	const [perPageEtags, setPerPageEtags] = createSignal<Record<number, string>>({});

	const [commentsData] = createResource(
		() => [props.id, currentPage(), highlight(), reload()] as const,
		async ([id, page]) => {
			const headers = stripNullish(
				perPageEtags()[page],
				(v) => ({
					"If-None-Match": v,
				}),
				undefined,
			);
			console.log("Headers are", headers);

			let data;

			const res = await GetApiComments(id, page, highlight(), headers);
			if (!res.ok) {
				throw new Error(res.error ?? res.statusText);
			}

			if (res.status === 304) {
				console.log("Cache hit");
				const cached = window.sessionStorage.getItem(perPageEtags()[page] ?? "");
				if (cached) {
					data = JSON.parse(cached) as typeof res.data;
					console.log("Loading from cache", page, data);
				} else {
					const newRes = await GetApiComments(id, page, highlight());
					if (newRes.ok && newRes.data) {
						data = newRes.data;
					}
				}
			} else {
				setPerPageEtags((old) => ({ ...old, [page]: res.headers.get("ETag") ?? "" }));
				data = res.data;
			}

			window.sessionStorage.setItem(perPageEtags()[page] ?? "", JSON.stringify(data));

			setUsername(res.headers.get("X-Username"));
			return data;
		},
	);

	let timeout: ReturnType<typeof setTimeout> | undefined;
	const changeHighlight = (idx: number) => {
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
		if (highlight() === 0) return;
		changeHighlight(highlight());
	});

	const comments = createMemo<(CommentDto & { key: number })[]>((prev) => {
		const data = commentsData();

		if (commentsData.state === "ready" && data) {
			return data.elements.map((val, key) => ({
				...val,
				key: data.total - currentPage() * data.perPage + (data.perPage - (key + 1)),
			}));
		}

		return prev ?? [];
	});
	const totalComments = () => commentsData()?.total ?? 0;
	const commentsPerPage = () => commentsData()?.perPage ?? 0;
	const totalPages = () => commentsData()?.pages ?? 0;

	onMount(() => {
		props.ref?.({
			submitted: () => {
				console.log("Submitted");
				setCurrentPage(1);
				setHighlight(totalComments() + 1);
			},
		});
	});

	const changePage = (page: number) => {
		if (page === currentPage()) {
			return;
		}
		setHighlight(0);
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
						onHighlightChange={setHighlight}
						onDelete={() => setReload(reload() + 1)}
						marked={c.key === highlight()}
						owner={username()}
						{...c}
					/>
				)}
			</For>
			{pagination()}
		</div>
	);
};
