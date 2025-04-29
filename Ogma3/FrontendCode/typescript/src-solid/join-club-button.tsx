import { PostApiClubjoin as joinClub, DeleteApiClubjoin as leaveClub } from "@g/paths-public";
import { log } from "@h/logger";
import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { createSignal } from "solid-js";

const JoinClubButton: ComponentType<{ clubId: number; csrf: string; isMember: boolean }> = (props) => {
	noShadowDOM();

	const [isMember, setIsMember] = createSignal(props.isMember);

	const join = async () => {
		const send = isMember() ? leaveClub : joinClub;

		const res = await send(
			{
				clubId: props.clubId,
			},
			{
				RequestVerificationToken: props.csrf,
			},
		);

		if (res.ok) {
			const data = res.data;
			setIsMember(data === true);
		} else {
			log.warn(res.statusText);
		}
	};

	return (
		<button
			type="button"
			class={`button max ${isMember() ? "leave" : "join"}`}
			title={isMember() ? "Leave" : "Join"}
			onClick={join}
		>
			{isMember() ? "Leave club" : "Join club"}
		</button>
	);
};

customElement(
	"o-join",
	{
		clubId: 0,
		csrf: "",
		isMember: false,
	},
	JoinClubButton,
);
