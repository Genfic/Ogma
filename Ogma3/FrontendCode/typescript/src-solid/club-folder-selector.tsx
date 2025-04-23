import {
	PostApiFoldersAddStory as addStoryToFolder,
	GetApiFolders as getFolders,
	GetApiClubsUser as getUserClubs,
} from "@g/paths-public";
import type { GetFolderResult, GetJoinedClubsResponse } from "@g/types-public";
import { log } from "@h/logger";
import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { For, createResource, createSignal, onMount, Switch, Match } from "solid-js";
import { Dialog, type DialogApi } from "./common/dialog";

const ClubFolderSelector: ComponentType<{ storyId: number; csrf: string }> = (
	props: { storyId: number; csrf: string },
	{ element },
) => {
	noShadowDOM();

	const [clubs, setClubs] = createSignal<GetJoinedClubsResponse[]>([]);
	const [selectedClub, setSelectedClub] = createSignal<GetJoinedClubsResponse | null>(null);
	const [status, setStatus] = createSignal({ message: "", success: false });
	const [selectedFolder, setSelectedFolder] = createSignal<GetFolderResult | null>(null);
	const [dialogRef, setDialogRef] = createSignal<DialogApi>();

	onMount(async () => {
		element.classList.add("wc-loaded");

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
				<Switch>
					<Match when={folders.loading}>
						<div>Loading folders...</div>
					</Match>
					<Match when={folders.error}>
						<div>Error fetching folders</div>
					</Match>
					<Match when={folders}>
						<For each={folders()} fallback={<div>This club has no folders</div>}>
							{(folder) => (
								<button
									type="button"
									classList={{
										locked: !folder.canAdd,
										active: selectedFolder()?.id === folder.id,
									}}
									class="folder"
									onClick={[select, folder]}
								>
									{folder.name}
								</button>
							)}
						</For>
					</Match>
				</Switch>
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
				<For each={clubs()} fallback={<div>You're not a member of any clubs</div>}>
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

	return (
		<>
			<button type="button" class="club-wc-button" onClick={() => dialogRef()?.open()}>
				Add to folder
			</button>

			<Dialog ref={setDialogRef} classes="club-folder-selector">
				<div class="content">{selectedClub() !== null ? selectedClubView() : allClubsView()}</div>
			</Dialog>
		</>
	);
};

customElement(
	"o-club-folder-selector",
	{
		storyId: 0,
		csrf: "",
	},
	ClubFolderSelector,
);
