import { DeleteApiClubjoin as leaveClub, PostApiClubjoin as joinClub } from "@g/paths-public";
import { log } from "@h/logger";
import { component } from "@h/web-components";
import { type ComponentType, noShadowDOM } from "solid-element";

const JoinClubButton: ComponentType<{ clubId: number; csrf: string; isMember: boolean }> = (props) => {
	noShadowDOM();

	let isMember = $signal(props.isMember);

	const join = async () => {
		const send = isMember ? leaveClub : joinClub;

		const res = await send(
			{
				clubId: props.clubId,
			},
			{
				RequestVerificationToken: props.csrf,
			},
		);

		if (res.ok) {
			isMember = res.data;
		} else {
			log.warn(res.data ?? res.statusText);
		}
	};

	return (
		<button
			type="button"
			class={`button max ${isMember ? "leave" : "join"}`}
			title={isMember ? "Leave" : "Join"}
			onClick={join}
		>
			{isMember ? "Leave club" : "Join club"}
		</button>
	);
};

component(
	"o-join",
	{
		clubId: 0,
		csrf: "",
		isMember: false,
	},
	JoinClubButton,
);
