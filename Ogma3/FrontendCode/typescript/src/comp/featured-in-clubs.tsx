import { GetApiClubsStory as getFeaturingClubs } from "@g/paths-public";
import type { GetClubsWithStoryResult } from "@g/types-public";
import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { createResource, For, Show } from "solid-js";
import { Dialog, type DialogApi } from "./common/_dialog";

const id = "featured-in-clubs";
const tagOverflow = 7;

const Club = ({ club }: { club: GetClubsWithStoryResult }) => (
	<a
		href={`/club/${club.id}/${club.name.toLowerCase().replace(" ", "-")}`}
		target="_blank"
		class="club"
		rel="noreferrer"
	>
		<img src={club.icon ?? "ph-250.png"} alt={club.name} width="48" height="48" />
		<span>{club.name}</span>
		<For each={club.folders.slice(0, tagOverflow)}>{(f) => <span class="folder">{f}</span>}</For>
		<Show when={club.folders.length > tagOverflow}>
			<span class="overflow">+ {club.folders.length - tagOverflow} more</span>
		</Show>
	</a>
);

const FeaturedInClubs: ComponentType<{ storyId: number }> = (props) => {
	noShadowDOM();

	let isOpen = $signal(false);
	const [clubs] = createResource($get(isOpen), async (condition: boolean) => {
		if (!condition) return null;
		const res = await getFeaturingClubs(props.storyId);
		return res.ok ? res.data : null;
	});
	const dialogRef = $signal<DialogApi>();

	const open = () => {
		isOpen = true;
		dialogRef?.open();
	};

	return (
		<>
			<button type="button" aria-controls={id} class="club-wc-button" onClick={open}>
				Featured in clubs
			</button>

			<Dialog
				ref={$set(dialogRef)}
				classes={["club-folder-selector"]}
				contentClass="clubs"
				header={<span>Featured in</span>}
			>
				<For each={clubs()} fallback={<div>This story hasn't been added to any clubs yet.</div>}>
					{(c) => <Club club={c} />}
				</For>
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
