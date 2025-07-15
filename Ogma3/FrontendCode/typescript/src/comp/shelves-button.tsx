import {
	DeleteApiShelfStories as removeFromShelf,
	GetApiShelfStories as getShelves,
	GetApiShelfStoriesQuick as getQuickShelves,
	PostApiShelfStories as addToShelf,
} from "@g/paths-public";
import type { GetCurrentUserQuickShelvesResult, GetPaginatedUserShelvesResult } from "@g/types-public";
import { useClickOutside } from "@h/click-outside";
import { log } from "@h/logger";
import { type ComponentType, customElement } from "solid-element";
import { createResource, For, type Setter, Show } from "solid-js";
import { Styled } from "./common/_styled";
import sharedCss from "./shared.css";
import css from "./shelves-button.css";

type Shelf = GetPaginatedUserShelvesResult & GetCurrentUserQuickShelvesResult;

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
	let more = $signal(false);
	let page = $signal(1);

	const [quickShelves, { mutate: mutateQuick }] = createResource(
		async () => {
			const res = await getQuickShelves(props.storyId);
			return res.ok ? (res.data as Shelf[]) : [];
		},
		{ initialValue: [] },
	);
	const [shelves, { mutate: mutateShelves }] = createResource(
		() => more,
		async (trigger) => {
			if (trigger) {
				const res = await getShelves(props.storyId, page);
				return res.ok ? (res.data as Shelf[]) : [];
			}
			return [];
		},
		{ initialValue: [] },
	);

	useClickOutside(element, () => (more = false));

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

	const iconStyle = (shelf: Shelf) => ({
		color: shelf.color ?? undefined,
	});

	const style = (shelf: Shelf) => ({
		"--s-col": shelf.doesContainBook ? (shelf.color ?? undefined) : undefined,
	});

	return () => (
		<>
			<For each={quickShelves()}>
				{(shelf) => (
					<button
						type="button"
						class="shelf action-btn"
						title={`Add to ${shelf.name}`}
						onClick={[addOrRemove, shelf.id]}
						style={style(shelf)}
					>
						{shelf.iconName ? (
							<o-icon style={iconStyle(shelf)} icon={shelf.iconName} />
						) : (
							<span style={iconStyle(shelf)}>B</span>
						)}
					</button>
				)}
			</For>

			<button type="button" title="All bookshelves" class="shelf action-btn" onClick={() => (more = !more)}>
				<o-icon class="material-icons-outlined" icon="lucide:ellipsis-vertical" />
			</button>

			<Show when={more}>
				<div class="more-shelves">
					<For each={shelves()}>
						{(shelf) => (
							<button
								type="button"
								class="shelf action-btn"
								title={`Add to ${shelf.name}`}
								onClick={[addOrRemove, shelf.id]}
								style={style(shelf)}
							>
								{shelf.iconName ? (
									<o-icon style={iconStyle(shelf)} icon={shelf.iconName} />
								) : (
									<span style={iconStyle(shelf)}>B</span>
								)}
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
