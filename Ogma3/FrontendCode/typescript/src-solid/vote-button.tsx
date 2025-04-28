import { DeleteApiVotes as deleteVote, GetApiVotes as getVotes, PostApiVotes as postVote } from "@g/paths-public";
import { log } from "@h/logger";
import { type ComponentType, customElement } from "solid-element";
import { createResource } from "solid-js";
import css from "./vote-button.css";
import { styled } from "@h/jsx-wc-style";

const VoteButton: ComponentType<{ storyId: number; csrf: string }> = (props) => {
	const [votes, { mutate }] = createResource(
		() => props.storyId,
		async (id) => {
			const result = await getVotes(id);
			return result.data;
		},
	);

	const vote = async () => {
		const send = votes().didVote ? deleteVote : postVote;

		const result = await send(
			{ storyId: props.storyId },
			{
				RequestVerificationToken: props.csrf,
			},
		);

		if (result.ok) {
			mutate(result.data);
		} else {
			log.error(`Error fetching data: ${result.statusText}`);
		}
	};

	return () => (
		<button
			type="button"
			class="votes"
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
	styled(css)(VoteButton),
);
