import { DeleteApiVotes as deleteVote, GetApiVotes as getVotes, PostApiVotes as postVote } from "@g/paths-public";
import { log } from "@h/logger";
import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { createSignal, onMount } from "solid-js";

const VoteButton: ComponentType<{ storyId: number; csrf: string }> = (props) => {
	noShadowDOM();

	const [voted, setVoted] = createSignal(false);
	const [score, setScore] = createSignal(0);

	onMount(async () => {
		const result = await getVotes(props.storyId);
		if (result.ok) {
			const { count, didVote } = result.data;
			setScore(count);
			setVoted(didVote);
		} else {
			log.error(`Error fetching data: ${result.statusText}`);
		}
	});

	const vote = async () => {
		const send = voted() ? deleteVote : postVote;

		const result = await send(
			{ storyId: props.storyId },
			{
				RequestVerificationToken: props.csrf,
			},
		);

		if (result.ok) {
			const { count, didVote } = result.data;
			setScore(count);
			setVoted(didVote);
		} else {
			log.error(`Error fetching data: ${result.statusText}`);
		}
	};

	return () => (
		<button
			type="button"
			class={`votes action-btn large ${voted() ? "active" : ""}`}
			onClick={vote}
			title="Give it a star!"
		>
			<o-icon icon={voted() ? "ic:round-star" : "ic:round-star-border"} class="material-icons-outlined" />
			<span class="count">{score() ?? 0}</span>
		</button>
	);
};

customElement(
	"o-vote",
	{
		storyId: 0,
		csrf: "",
	},
	VoteButton,
);
