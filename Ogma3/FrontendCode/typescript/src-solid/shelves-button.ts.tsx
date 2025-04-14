import {
	PostApiShelfStories as addToShelf,
	GetApiShelfStoriesQuick as getQuickShelves,
	GetApiShelfStories as getShelves,
	DeleteApiShelfStories as removeFromShelf,
} from "@g/paths-public";
import { clickOutsideSolid } from "@h/click-outside";
import { log } from "@h/logger";
import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { For, Show, createSignal, onMount } from "solid-js";
import type { ShelfResult } from "@g/types-public";

const ShelvesButton: ComponentType<{ storyId: number; csrf: string }> = (props, { element }) => {
	noShadowDOM();

	const [quickShelves, setQuickShelves] = createSignal<ShelfResult[]>([]);
	const [shelves, setShelves] = createSignal<ShelfResult[]>([]);
	const [more, setMore] = createSignal(false);
	const [page, setPage] = createSignal(1);
	const [moreShelvesLoaded, setMoreShelvesLoaded] = createSignal(false);

	onMount(async () => {
		await getQuickShelvesData();
		element.classList.add("wc-loaded");

		clickOutsideSolid(element, () => {
			setMore(false);
		});
	});

	const iconStyle = (shelf: ShelfResult) => ({
		color: shelf.color,
	});

	const getQuickShelvesData = async () => {
		const res = await getQuickShelves(props.storyId);
		if (res.ok) {
			setQuickShelves(res.data);
		} else {
			log.error(res.statusText);
		}
	};

	const getShelvesData = async () => {
		const res = await getShelves(props.storyId, page());
		if (res.ok) {
			setShelves(res.data);
			setMoreShelvesLoaded(true);
		} else {
			log.error(res.statusText);
		}
	};

	const showMore = async () => {
		setMore(!more());
		if (more() && !moreShelvesLoaded()) {
			await getShelvesData();
		}
	};

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
			const data = res.data;
			// Update the shelf data
			const updatedQuickShelves = [...quickShelves()];
			const quickIndex = updatedQuickShelves.findIndex((s) => s.id === data.shelfId);
			if (quickIndex >= 0) {
				updatedQuickShelves[quickIndex] = {
					...updatedQuickShelves[quickIndex],
					doesContainBook: !exists,
				};
				setQuickShelves(updatedQuickShelves);
			}

			const updatedShelves = [...shelves()];
			const shelfIndex = updatedShelves.findIndex((s) => s.id === data.shelfId);
			if (shelfIndex >= 0) {
				updatedShelves[shelfIndex] = {
					...updatedShelves[shelfIndex],
					doesContainBook: !exists,
				};
				setShelves(updatedShelves);
			}
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
						onClick={() => addOrRemove(shelf.id)}
						style={{ "box-shadow": shelf.doesContainBook ? `${shelf.color} inset 0 0 0 3px` : null }}
					>
						<o-icon class="material-icons-outlined" style={iconStyle(shelf)} icon={shelf.iconName} />
					</button>
				)}
			</For>

			<button type="button" title="All bookshelves" class="shelf action-btn" onClick={showMore}>
				<o-icon class="material-icons-outlined" icon="lucide:ellipsis-vertical" />
			</button>

			<Show when={more()}>
				<div class="more-shelves">
					<For each={shelves()}>
						{(shelf) => (
							<button
								type="button"
								class="action-btn"
								title={`Add to ${shelf.name}`}
								onClick={() => addOrRemove(shelf.id)}
								style={{
									"box-shadow": shelf.doesContainBook ? `${shelf.color} inset 0 0 0 3px` : null,
								}}
							>
								<o-icon
									class="material-icons-outlined"
									style={iconStyle(shelf)}
									icon={shelf.iconName}
								/>
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
	ShelvesButton,
);
