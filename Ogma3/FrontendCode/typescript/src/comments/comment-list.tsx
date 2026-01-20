import { GetApiComments, GetApiCommentsLocate } from "@g/paths-public";
import { DateSafeJsonParse } from "@g/typed-fetch";
import type { CommentDto } from "@g/types-public";
import { stripNullish } from "@h/csharping";
import { type Component, createEffect, createMemo, createResource, createSignal, For, onMount } from "solid-js";
import { Comment } from "./comment";
import { CommentListPagination } from "./comment-list-pagination";

export interface CommentListFunctions {
	submitted: (id: string) => void;
}

interface Props {
	id: number;
	ref?: (functions: CommentListFunctions) => void;
}

type SuccessDataFrom<T> = T extends Promise<infer R> ? (R extends { ok: true; data: infer D } ? D : never) : never;
type CommentsData = SuccessDataFrom<ReturnType<typeof GetApiComments>>;

export const CommentList: Component<Props> = (props) => {
	const [currentPage, setCurrentPage] = createSignal(1);
	const [highlight, setHighlight] = createSignal("");
	const [reload, setReload] = createSignal("");
	const [username, setUsername] = createSignal<string | null>(null);
	const [deleted, setDeleted] = createSignal<string[]>([]);
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

			const res = await GetApiComments(id, page, headers);
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
					const newRes = await GetApiComments(id, page);
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

	createEffect(() => {
		const targetId = highlight();
		if (commentsData.state === "ready" && targetId.length > 0) {
			const element = document.getElementById(`comment-${targetId}`);

			if (element) {
				element.scrollIntoView({
					behavior: "smooth",
					block: "center",
					inline: "nearest",
				});
			}
		}
	});

	const changeHighlight = (e: MouseEvent, id: string) => {
		e.preventDefault();
		setHighlight(id);

		if (id.length <= 0) return;

		document.getElementById(`comment-${id}`)?.scrollIntoView({
			behavior: "smooth",
			block: "center",
			inline: "nearest",
		});

		history.replaceState(undefined, "", `#comment-${id}`);
	};

	const comments = createMemo<CommentDto[]>((prev) => {
		const data = commentsData();
		if (commentsData.state === "ready" && data) {
			return data.elements.map(
				(c) => ({ ...c, deletedBy: deleted().includes(c.id) ? "User" : c.deletedBy }) as CommentDto,
			);
		}
		return prev ?? [];
	});

	const totalComments = () => commentsData()?.total ?? 0;
	const commentsPerPage = () => commentsData()?.perPage ?? 0;
	const totalPages = () => commentsData()?.pages ?? 0;

	onMount(() => {
		props.ref?.({
			submitted: (id: string) => {
				setCurrentPage(1);
				setHighlight(id);
				setReload((r) => r + 1);
			},
		});
	});

	// Handle Deep Linking
	onMount(async () => {
		const hash = window.location.hash;
		const match = hash.match(/^#comment-([a-zA-Z0-9]+)$/);

		if (!match || !match[1]) return;

		setHighlight(match[1]);

		const res = await GetApiCommentsLocate(props.id, match[1]);
		if (res.ok) {
			setCurrentPage(res.data.page);
		} else {
			console.error(res.error);
		}
	});

	const changePage = (page: number) => {
		if (page === currentPage()) return;
		setHighlight("");
		setCurrentPage(page);
	};

	const deleteComment = (id: string) => {
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
						onHighlightChange={(e) => changeHighlight(e, c.id)}
						onDelete={() => deleteComment(c.id)}
						marked={c.id === highlight()}
						owner={username()}
						{...c}
					/>
				)}
			</For>
			{pagination()}
		</div>
	);
};
