import { GetApiClubsStory as getFeaturingClubs } from "@g/paths-public";
import type { GetClubsWithStoryResult } from "@g/types-public";
import { customElement } from "solid-element";
import { For, Show, createSignal, onMount } from "solid-js";

customElement(
	"o-featured-in-clubs",
	{
		storyId: Number,
	},
	(props) => {
		const [visible, setVisible] = createSignal(false);
		const [clubs, setClubs] = createSignal<GetClubsWithStoryResult[]>([]);

		onMount(async () => {
			props.element.classList.add("wc-loaded");
			await fetch();
		});

		const fetch = async () => {
			const response = await getFeaturingClubs(props.storyId);
			if (response.ok) {
				setClubs(response.data);
			}
		};

		const open = async () => {
			setVisible(true);
			await fetch();
		};

		const clubsView = () => (
			<Show when={clubs().length > 0} fallback={<div>This story hasn't been added to any clubs yet.</div>}>
				<div class="clubs">
					<For each={clubs()}>
						{(c) => (
							<a
								href={`/club/${c.id}/${c.name.toLowerCase().replace(" ", "-")}`}
								target="_blank"
								class="club"
								rel="noreferrer"
							>
								<img src={c.icon ?? "ph-250.png"} alt={c.name} width="48" height="48" />
								<span>{c.name}</span>
								<For each={c.folders.slice(0, 5)}>{(f) => <span class="folder">{f}</span>}</For>
								<Show when={c.folders.length > 5}>
									<span class="overflow">+ {c.folders.length - 5} more</span>
								</Show>
							</a>
						)}
					</For>
				</div>
			</Show>
		);

		return () => (
			<>
				<button type="button" class="club-wc-button" onClick={open}>
					Featured in clubs
				</button>

				<Show when={visible()}>
					<div class="club-folder-selector my-modal" onClick={() => setVisible(false)}>
						<div class="content" onClick={(e) => e.stopPropagation()}>
							<div class="header">
								<span>Featured in</span>
							</div>

							{clubsView()}
						</div>
					</div>
				</Show>
			</>
		);
	},
);
