import {
	PostApiShelfStories as addToShelf,
	GetApiShelfStoriesQuick as getQuickShelves,
	GetApiShelfStories as getShelves,
	DeleteApiShelfStories as removeFromShelf,
} from "@g/paths-public";
import { useClickOutside } from "@h/click-outside";
import { log } from "@h/logger";
import { type ComponentType, customElement } from "solid-element";
import { For, Show, createSignal, createResource, type Setter } from "solid-js";
import type { QuickShelvesResult, ShelfResult } from "@g/types-public";
import { Styled } from "./common/_styled";
import css from "./shelves-button.css";
import sharedCss from "./shared.css";

type Shelf = ShelfResult & QuickShelvesResult;

const updateShelfData = (
	currentShelves: Shelf[],
	setShelvesFunc: Setter<Shelf[]>,
	updatedShelfId: number,
	containsBook: boolean,
) => {
	const newShelves = currentShelves.map((shelf) => {
		if (shelf.id === updatedShelfId) {
			return {
				...shelf,
				doesContainBook: containsBook,
			};
		}
		return shelf;
	});
	setShelvesFunc(newShelves);
};

const ShelvesButton: ComponentType<{ storyId: number; csrf: string }> = (props, { element }) => {
	const [more, setMore] = createSignal(false);
	const [page, setPage] = createSignal(1);

	const [quickShelves, { mutate: mutateQuick }] = createResource(async () => {
		const res = await getQuickShelves(props.storyId);
		return res.ok ? (res.data as Shelf[]) : null;
	});
	const [shelves, { mutate: mutateShelves }] = createResource(
		more,
		async (trigger) => {
			if (trigger) {
				const res = await getShelves(props.storyId, page());
				return res.ok ? (res.data as Shelf[]) : null;
			}
			return null;
		},
		{ initialValue: [] },
	);

	useClickOutside(element, () => setMore(false));

	const iconStyle = (shelf: ShelfResult) => ({
		color: shelf.color,
	});

	const addOrRemove = async (id: number) => {
		const allShelves = [...shelves(), ...quickShelves()];
		const exists = allShelves.some((s) => s.doesContainBook && s.id === id);
		const send = exists ? removeFromShelf : addToShelf;

		const res = await send(
			{
				storyId: props.storyId,
				shelfId: id,
			},
			{
				RequestVerificationToken: props.csrf,
			},
		);

		if (res.ok) {
			const { shelfId } = res.data;

			updateShelfData(quickShelves(), mutateQuick, shelfId, !exists);
			updateShelfData(shelves(), mutateShelves, shelfId, !exists);
		} else {
			log.error(res.statusText);
		}
	};

	return () => (
		<>
			<For each={quickShelves()}>
				{(shelf) => (
					<button
						type="button"
						class="shelf action-btn"
						title={`Add to ${shelf.name}`}
						onClick={[addOrRemove, shelf.id]}
						style={{
							"--s-col": shelf.doesContainBook && shelf.color,
						}}
					>
						<o-icon style={iconStyle(shelf)} icon={shelf.iconName} />
					</button>
				)}
			</For>

			<button type="button" title="All bookshelves" class="shelf action-btn" onClick={[setMore, !more()]}>
				<o-icon class="material-icons-outlined" icon="lucide:ellipsis-vertical" />
			</button>

			<Show when={more()}>
				<div class="more-shelves">
					<For each={shelves()}>
						{(shelf) => (
							<button
								type="button"
								class="shelf action-btn"
								title={`Add to ${shelf.name}`}
								onClick={[addOrRemove, shelf.id]}
								style={{
									"--s-col": shelf.doesContainBook && shelf.color,
								}}
							>
								<o-icon class="icon" style={iconStyle(shelf)} icon={shelf.iconName} />
								<span>{shelf.name}</span>
							</button>
						)}
					</For>
				</div>
			</Show>
		</>
	);
};

customElement(
	"o-shelves",
	{
		storyId: 0,
		csrf: "",
	},
	Styled(ShelvesButton, sharedCss, css),
);
