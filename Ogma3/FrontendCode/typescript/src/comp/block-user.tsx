import { DeleteApiUsersBlock as unblockUser, PostApiUsersBlock as blockUser } from "@g/paths-public";
import { log } from "@h/logger";
import { component } from "@h/web-components";
import { type Component, createSignal } from "solid-js";
import { IcRoundBlock } from "../icons/IcRoundBlock";
import css from "./block-user.css";

const BlockUser: Component<{ userName: string; csrf: string; isBlocked: boolean }> = (props) => {
	// noShadowDOM();

	const [isBlocked, setIsBlocked] = createSignal(props.isBlocked);

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
			log.warn(res.error);
		}
	};

	return (
		<button class="block-btn" type="button" onClick={block}>
			<IcRoundBlock />
			{isBlocked() ? "Unblock" : "Block"}
		</button>
	);
};

component(
	"o-block",
	{
		userName: "",
		csrf: "",
		isBlocked: false,
	},
	BlockUser,
	css,
);
