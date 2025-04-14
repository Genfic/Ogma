import { PostApiUsersFollow as followUser, DeleteApiUsersFollow as unfollowUser } from "@g/paths-public";
import { log } from "@h/logger";
import { customElement } from "solid-element";
import { createSignal, onMount } from "solid-js";

customElement(
	"o-follow",
	{
		userName: String,
		csrf: String,
		isFollowed: Boolean,
	},
	(props) => {
		const [isFollowed, setIsFollowed] = createSignal(props.isFollowed);

		onMount(() => {
			props.element.classList.add("wc-loaded");
		});

		const follow = async () => {
			const send = isFollowed() ? unfollowUser : followUser;

			const res = await send(
				{
					name: props.userName,
				},
				{
					RequestVerificationToken: props.csrf,
				},
			);

			if (res.ok) {
				setIsFollowed(res.data);
			} else {
				log.warn(res.statusText);
			}
		};

		return () => (
			<button
				type="button"
				class={`button max ${isFollowed() ? "leave" : "join"}`}
				title={isFollowed() ? "Unfollow" : "Follow"}
				onClick={follow}
			>
				{isFollowed() ? "Following" : "Follow"}
			</button>
		);
	},
);
