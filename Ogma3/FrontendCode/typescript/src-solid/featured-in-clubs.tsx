import { GetApiClubsStory as getFeaturingClubs } from "@g/paths-public";
import type { GetClubsWithStoryResult } from "@g/types-public";
import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { For, Show, createSignal, createResource } from "solid-js";
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

	const [isOpen, setIsOpen] = createSignal(false);
	const [clubs] = createResource(isOpen, async (condition: boolean) => {
		if (!condition) return null;
		const res = await getFeaturingClubs(props.storyId);
		return res.ok ? res.data : null;
	});
	const [dialogRef, setDialogRef] = createSignal<DialogApi>();

	const open = () => {
		setIsOpen(true);
		dialogRef().open();
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

					<div class="clubs">
						<For each={clubs()} fallback={<div>This story hasn't been added to any clubs yet.</div>}>
							{(c) => <Club club={c} />}
						</For>
					</div>
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
