import { DeleteApiUsersBlock as unblockUser, PostApiUsersBlock as blockUser } from "@g/paths-public";
import { log } from "@h/logger";
import { customElement } from "solid-element";
import { type Component, createSignal } from "solid-js";
import { IcRoundBlock } from "../icons/IcRoundBlock";
import css from "./block-user.css";
import { Styled } from "./common/_styled";

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

customElement(
	"o-block",
	{
		userName: "",
		csrf: "",
		isBlocked: false,
	},
	Styled(BlockUser, css),
);
