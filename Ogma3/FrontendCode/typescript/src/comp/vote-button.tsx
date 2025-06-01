import { DeleteApiVotes as deleteVote, GetApiVotes as getVotes, PostApiVotes as postVote } from "@g/paths-public";
import { log } from "@h/logger";
import { type ComponentType, customElement } from "solid-element";
import { createResource } from "solid-js";
import { Styled } from "./common/_styled";
import sharedCss from "./shared.css";
import css from "./vote-button.css";

const VoteButton: ComponentType<{ storyId: number; csrf: string }> = (props) => {
	const [votes, { mutate }] = createResource(
		() => props.storyId,
		async (id) => {
			const result = await getVotes(id);
			if (result.ok) {
				return result.data;
			}
			throw new Error(result.error);
		},
	);

	const vote = async () => {
		const send = votes()?.didVote ? deleteVote : postVote;

		const result = await send(
			{ storyId: props.storyId },
			{
				RequestVerificationToken: props.csrf,
			},
		);

		if (result.ok) {
			mutate(result.data);
		} else {
			log.error(`Error fetching data: ${result.error}`);
		}
	};

	return () => (
		<button
			type="button"
			class="votes action-btn"
			classList={{ active: votes()?.didVote }}
			onClick={vote}
			title="Give it a star!"
		>
			<o-icon icon={votes()?.didVote ? "ic:round-star" : "ic:round-star-border"} />
			<span class="count">{votes()?.count ?? 0}</span>
		</button>
	);
};

customElement(
	"o-vote",
	{
		storyId: 0,
		csrf: "",
	},
	Styled(VoteButton, sharedCss, css),
);
