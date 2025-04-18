import { PostApiUsersBlock as blockUser, DeleteApiUsersBlock as unblockUser } from "@g/paths-public";
import { log } from "@h/logger";
import { customElement, noShadowDOM } from "solid-element";
import { type Component, createSignal } from "solid-js";

const BlockUser: Component<{ userName: string; csrf: string; isBlocked: boolean }> = (props) => {
	noShadowDOM();

	const [ isBlocked, setIsBlocked ] = createSignal(props.isBlocked);

	const block = async () => {
		const send = isBlocked() ? unblockUser : blockUser;

		const res = await send(
			{
				name: props.userName,
			},
			{
				RequestVerificationToken: props.csrf,
			},
		);

		if (res.ok) {
			setIsBlocked(res.data);
		} else {
			log.warn(res.statusText);
		}
	};

	return (
		<button type="button" onClick={block}>
			{isBlocked() ? "Unblock" : "Block"}
		</button>
	);
};

customElement(
	"o-block",
	{
		userName: "",
		csrf: "",
		isBlocked: false,
	},
	BlockUser,
);