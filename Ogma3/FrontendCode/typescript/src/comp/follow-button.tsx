import { DeleteApiUsersFollow as unfollowUser, PostApiUsersFollow as followUser } from "@g/paths-public";
import { log } from "@h/logger";
import { component } from "@h/web-components";
import { type ComponentType, noShadowDOM } from "solid-element";
import { createSignal } from "solid-js";

const FollowButton: ComponentType<{ userName: string; csrf: string; isFollowed: boolean }> = (props) => {
	noShadowDOM();

	const [isFollowed, setIsFollowed] = createSignal(props.isFollowed);

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

	return (
		<button
			type="button"
			class={`button max ${isFollowed() ? "leave" : "join"}`}
			title={isFollowed() ? "Unfollow" : "Follow"}
			onClick={follow}
		>
			{isFollowed() ? "Following" : "Follow"}
		</button>
	);
};

component(
	"o-follow",
	{
		userName: "",
		csrf: "",
		isFollowed: false,
	},
	FollowButton,
);
