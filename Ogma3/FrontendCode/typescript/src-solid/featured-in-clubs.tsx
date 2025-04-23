import { GetApiClubsStory as getFeaturingClubs } from "@g/paths-public";
import type { GetClubsWithStoryResult } from "@g/types-public";
import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { For, Show, createSignal, onMount } from "solid-js";
import { Dialog, type DialogApi } from "./common/dialog";

const id = "featured-in-clubs";
const tagOverflow = 7;

const FeaturedInClubs: ComponentType<{ storyId: number }> = (props, { element }) => {
	noShadowDOM();

	const [clubs, setClubs] = createSignal<GetClubsWithStoryResult[]>([]);
	const [dialogRef, setDialogRef] = createSignal<DialogApi>();

	onMount(() => {
		element.classList.add("wc-loaded");
		if (!dialogRef) {
			return;
		}
	});

	const clubsView = () => (
		<div class="clubs">
			<For each={clubs()} fallback={<div>This story hasn't been added to any clubs yet.</div>}>
				{(c) => (
					<a
						href={`/club/${c.id}/${c.name.toLowerCase().replace(" ", "-")}`}
						target="_blank"
						class="club"
						rel="noreferrer"
					>
						<img src={c.icon ?? "ph-250.png"} alt={c.name} width="48" height="48" />
						<span>{c.name}</span>
						<For each={c.folders.slice(0, tagOverflow)}>{(f) => <span class="folder">{f}</span>}</For>
						<Show when={c.folders.length > tagOverflow}>
							<span class="overflow">+ {c.folders.length - tagOverflow} more</span>
						</Show>
					</a>
				)}
			</For>
		</div>
	);

	const open = async () => {
		const response = await getFeaturingClubs(props.storyId);
		if (response.ok) {
			setClubs(response.data);
		}

		dialogRef()?.open();
	};

	return (
		<>
			<button type="button" aria-controls={id} class="club-wc-button" onClick={open}>
				Featured in clubs
			</button>

			<Dialog ref={setDialogRef} classes="club-folder-selector">
				<div class="content">
					<div class="header">
						<span>Featured in</span>
					</div>

					{clubsView()}
				</div>
			</Dialog>
		</>
	);
};

customElement(
	"o-featured-in-clubs",
	{
		storyId: 0,
	},
	FeaturedInClubs,
);
