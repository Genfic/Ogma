import { GetApiClubsUser as getUserClubs, GetApiFolders as getFolders, PostApiFoldersAddStory as addStoryToFolder } from "@g/paths-public";
import type { GetFolderResult, GetJoinedClubsResponse } from "@g/types-public";
import { component } from "@h/web-components";
import { type ComponentType, noShadowDOM } from "solid-element";
import { createResource, For, Match, Switch } from "solid-js";
import { Dialog, type DialogApi } from "./common/_dialog";

const ClubFolderSelector: ComponentType<{ storyId: number; csrf: string }> = (props) => {
	noShadowDOM();

	let isOpen = $signal(false);
	let selectedClub = $signal<GetJoinedClubsResponse | null>(null);
	let status = $signal({ message: "", success: false });
	let selectedFolder = $signal<GetFolderResult | null>(null);
	let dialogRef = $signal<DialogApi>();

	const [clubs] = createResource($get(isOpen), async (condition) => {
		if (!condition) return null;
		const res = await getUserClubs();
		return res.ok ? res.data : null;
	});

	const [folders] = createResource($get(selectedClub), async (club) => {
		if (!club) {
			throw new Error("Club not selected");
		}
		const res = await getFolders(club.id);
		if (!res.ok) {
			throw new Error(res.statusText);
		}
		return res.data;
	});

	const reset = () => {
		selectedClub = null;
		selectedFolder = null;
		status = { message: "", success: false };
	};

	const open = () => {
		isOpen = true;
		dialogRef?.open();
	};

	const setClub = (club: GetJoinedClubsResponse | null) => {
		selectedClub = club;
		selectedFolder = null;
	};

	const add = async () => {
		const folder = selectedFolder;

		if (!folder) {
			status = {
				message: "You must select a folder!",
				success: false,
			};
			return;
		}

		const response = await addStoryToFolder(
			{
				folderId: folder.id,
				storyId: props.storyId,
			},
			{
				RequestVerificationToken: props.csrf,
			},
		);

		if (!response.ok) {
			status = {
				message: response.data ?? response.statusText,
				success: false,
			};
			return;
		}

		status = {
			message: "Successfully added",
			success: true,
		};
	};

	const selectedClubView = () => (
		<>
			<div class={`msg ${status.success ? "success" : "error"}`}>{status.message}</div>

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
										active: selectedFolder?.id === folder.id,
									}}
									class="folder"
									onClick={() => (selectedFolder = folder)}
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
				<button type="button" class="active-border cancel" onClick={[setClub, null]}>
					Go back
				</button>
			</div>
		</>
	);

	const allClubsView = () => (
		<div class="clubs">
			<For each={clubs()} fallback={<div>You're not a member of any clubs</div>}>
				{(club) => (
					<button type="button" class="club" onClick={[setClub, club]}>
						<img src={club.icon ?? "ph-250.png"} alt={club.name} width="48" height="48" />
						<span>{club.name}</span>
					</button>
				)}
			</For>
		</div>
	);

	const selectedView = () => {
		const club = selectedClub;

		if (!club) {
			return {
				view: allClubsView,
				head: <span>Your clubs</span>,
			};
		}

		return {
			view: selectedClubView,
			head: (
				<>
					<img src={club.icon ?? "ph-250.png"} alt={club.name} width="32" height="32" />
					<span>{club.name}</span>
				</>
			),
		};
	};

	return (
		<>
			<button type="button" class="club-wc-button" onClick={open}>
				Add to folder
			</button>

			<Dialog ref={$set(dialogRef)} onClose={reset} header={selectedView().head} classes={["club-folder-selector"]}>
				<div class="content">{selectedView().view()}</div>
			</Dialog>
		</>
	);
};

component(
	"o-club-folder-selector",
	{
		storyId: 0,
		csrf: "",
	},
	ClubFolderSelector,
);
