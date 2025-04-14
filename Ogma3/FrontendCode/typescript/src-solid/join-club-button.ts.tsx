import { PostApiClubjoin as joinClub, DeleteApiClubjoin as leaveClub } from "@g/paths-public";
import { log } from "@h/logger";
import { customElement } from "solid-element";
import { createSignal, onMount } from "solid-js";

customElement(
	"o-join",
	{
		clubId: Number,
		csrf: String,
		isMember: {
			type: Boolean,
			default: false,
		},
	},
	(props) => {
		const [isMember, setIsMember] = createSignal(props.isMember);

		onMount(() => {
			props.element.classList.add("wc-loaded");
		});

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

		return () => (
			<button
				type="button"
				class={`button max ${isMember() ? "leave" : "join"}`}
				title={isMember() ? "Leave" : "Join"}
				onClick={join}
			>
				{isMember() ? "Leave club" : "Join club"}
			</button>
		);
	},
);
