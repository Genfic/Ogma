import { PostApiUsersBlock as blockUser, DeleteApiUsersBlock as unblockUser } from "@g/paths-public";
import { log } from "@h/logger";
import { customElement } from "solid-element";
import { createSignal } from "solid-js";

customElement(
	"o-block",
	{
		userName: String,
		csrf: String,
		isBlocked: Boolean,
	},
	(props) => {
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
				log.warn(res.statusText);
			}
		};

		return () => (
			<button type="button" onClick={block}>
				{isBlocked() ? "Unblock" : "Block"}
			</button>
		);
	},
);
