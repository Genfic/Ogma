import {
	PostApiFoldersAddStory as addStoryToFolder,
	GetApiFolders as getFolders,
	GetApiClubsUser as getUserClubs,
} from "@g/paths-public";
import type { GetFolderResult, GetJoinedClubsResponse } from "@g/types-public";
import { log } from "@h/logger";
import { customElement } from "solid-element";
import { For, Show, createResource, createSignal, onMount } from "solid-js";

customElement(
	"o-club-folder-selector",
	{
		storyId: Number,
		csrf: String,
	},
	(props) => {
		const [clubs, setClubs] = createSignal<GetJoinedClubsResponse[]>([]);
		const [selectedClub, setSelectedClub] = createSignal<GetJoinedClubsResponse | null>(null);
		const [status, setStatus] = createSignal({ message: "", success: false });
		const [visible, setVisible] = createSignal(false);
		const [selectedFolder, setSelectedFolder] = createSignal<GetFolderResult | null>(null);

		onMount(async () => {
			props.element.classList.add("wc-loaded");

			const response = await getUserClubs();
			if (response.ok) {
				setClubs(response.data);
			} else {
				log.error(`Error fetching data: ${response.statusText}`);
			}
		});

		// Use resource for folders
		const [folders] = createResource(selectedClub, async (club) => {
			if (!club) {
				throw new Error("Club not selected");
			}
			const res = await getFolders(club.id);
			if (!res.ok) {
				throw new Error(res.statusText);
			}
			return res.data;
		});

		const setClub = (club: GetJoinedClubsResponse | null) => {
			setSelectedClub(club);
			setSelectedFolder(null);
		};

		const setVisibility = (visibility: boolean) => {
			setVisible(visibility);
		};

		const select = (folder: GetFolderResult) => {
			log.info(`Selecting folder with ID ${folder.id}`);
			setSelectedFolder(folder);
		};

		const add = async () => {
			if (selectedFolder() === null) {
				setStatus({
					message: "You must select a folder!",
					success: false,
				});
				return;
			}

			const response = await addStoryToFolder(
				{
					folderId: selectedFolder().id,
					storyId: props.storyId,
				},
				{
					RequestVerificationToken: props.csrf,
				},
			);

			if (response.ok) {
				setStatus({
					message: "Successfully added",
					success: true,
				});
			} else if (typeof response.data === "string") {
				setStatus({
					message: response.data.replaceAll('"', ""),
					success: false,
				});
			}
		};

		const selectedClubView = () => (
			<>
				<div class="header">
					<img src={selectedClub().icon ?? "ph-250.png"} alt={selectedClub().name} width="32" height="32" />
					<span>{selectedClub().name}</span>
				</div>

				<div class={`msg ${status().success ? "success" : "error"}`}>{status().message}</div>

				<div class="folders">
					<Show when={!folders.loading} fallback={<div>Loading folders...</div>}>
						<Show when={!folders.error} fallback={<div>Error: {folders.error?.message}</div>}>
							<For each={folders()}>
								{(folder) => (
									<button
										type="button"
										class={`folder ${!folder.canAdd ? "locked" : ""} ${
											selectedFolder()?.id === folder.id ? "active" : ""
										}`}
										onClick={() => select(folder)}
									>
										{folder.name}
									</button>
								)}
							</For>
						</Show>
					</Show>
				</div>

				<div class="buttons">
					<button type="button" class="active-border add" onClick={add}>
						Add
					</button>
					<button type="button" class="active-border cancel" onClick={() => setClub(null)}>
						Go back
					</button>
				</div>
			</>
		);

		const allClubsView = () => (
			<>
				<div class="header">
					<span>Your clubs</span>
				</div>

				<div class="clubs">
					<For each={clubs()}>
						{(c) => (
							<button type="button" class="club" onClick={() => setClub(c)}>
								<img src={c.icon ?? "ph-250.png"} alt={c.name} width="48" height="48" />
								<span>{c.name}</span>
							</button>
						)}
					</For>
				</div>
			</>
		);

		return () => (
			<>
				<button type="button" class="club-wc-button" onClick={() => setVisibility(true)}>
					Add to folder
				</button>

				<Show when={visible()}>
					<div class="club-folder-selector my-modal" onClick={() => setVisibility(false)}>
						<div class="content" onClick={(e) => e.stopPropagation()}>
							{selectedClub() !== null ? selectedClubView() : allClubsView()}
						</div>
					</div>
				</Show>
			</>
		);
	},
);
