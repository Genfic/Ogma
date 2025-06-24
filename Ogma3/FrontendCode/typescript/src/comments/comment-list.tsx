import { GetApiComments } from "@g/paths-public";
import { DateSafeJsonParse } from "@g/typed-fetch";
import type { CommentDto } from "@g/types-public";
import { stripNullish } from "@h/csharping";
import { type Component, createMemo, createResource, createSignal, For, onMount } from "solid-js";
import { Comment } from "./comment";
import { CommentListPagination } from "./comment-list-pagination";

export interface CommentListFunctions {
	submitted: () => void;
}

interface Props {
	id: number;
	ref?: (functions: CommentListFunctions) => void;
}

type SuccessDataFrom<T> = T extends Promise<infer R> ? (R extends { ok: true; data: infer D } ? D : never) : never;

type CommentsData = SuccessDataFrom<ReturnType<typeof GetApiComments>>;

export const CommentList: Component<Props> = (props) => {
	const [currentPage, setCurrentPage] = createSignal(1);
	const [highlight, setHighlight] = createSignal(Number(window.location.hash.match(/^#comment-(\d+)$/)?.[1]) || 0);
	const [reload, setReload] = createSignal(0);
	const [username, setUsername] = createSignal<string | null>(null);
	const [deleted, setDeleted] = createSignal<number[]>([]);

	const [perPageEtags, setPerPageEtags] = createSignal<Record<number, string>>({});

	const [commentsData] = createResource(
		() => [props.id, currentPage(), reload()] as const,
		async ([id, page]) => {
			const headers = stripNullish(
				perPageEtags()[page],
				(v) => ({
					"If-None-Match": v,
				}),
				undefined,
			);

			const res = await GetApiComments(id, page, highlight(), headers);
			if (!res.ok) {
				throw new Error(res.error ?? res.statusText);
			}

			let data: CommentsData;
			if (res.status === 304) {
				console.log("Cache hit");
				const cached = window.sessionStorage.getItem(perPageEtags()[page] ?? "");
				if (cached) {
					data = DateSafeJsonParse<CommentsData>(cached);
					console.log("Loading from cache", page, data);
				} else {
					const newRes = await GetApiComments(id, page, highlight());
					if (newRes.ok) {
						data = newRes.data;
					} else {
						throw new Error(newRes.error);
					}
				}
			} else {
				setPerPageEtags((old) => ({ ...old, [page]: res.headers.get("ETag") ?? "" }));
				data = res.data;

				setUsername(res.headers.get("X-Username"));
			}

			window.sessionStorage.setItem(perPageEtags()[page] ?? "", JSON.stringify(data));
			return data;
		},
	);

	const changeHighlight = (e: MouseEvent, idx: number) => {
		e.preventDefault();
		setHighlight(idx);

		if (idx === 0) {
			return;
		}

		document.getElementById(`comment-${idx}`)?.scrollIntoView({
			behavior: "smooth",
			block: "center",
			inline: "nearest",
		});

		history.replaceState(undefined, "", `#comment-${idx}`);
	};

	const comments = createMemo<(CommentDto & { key: number })[]>((prev) => {
		const data = commentsData();

		if (commentsData.state === "ready" && data) {
			return data.elements
				.map((c) => ({ ...c, deletedBy: deleted().includes(c.id) ? "User" : c.deletedBy }) as CommentDto)
				.map((val, key) => ({
					...val,
					key: data.total - ((currentPage() - 1) * data.perPage + key),
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

	const deleteComment = (id: number) => {
		setReload(id);
		setDeleted([...deleted(), id]);
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
						onHighlightChange={changeHighlight}
						onDelete={() => deleteComment(c.id)}
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
