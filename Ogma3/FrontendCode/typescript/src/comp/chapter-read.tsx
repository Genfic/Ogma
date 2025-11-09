import { DeleteApiChaptersread, PostApiChaptersread } from "@g/paths-public";
import { component } from "@h/web-components";
import type { ComponentType } from "solid-element";
import { LucideEye } from "../icons/LucideEye";
import { LucideEyeClosed } from "../icons/LucideEyeClosed";
import css from "./chapter-read.css";
import { useChaptersRead } from "./common/_chaptersReadData";

const ChapterRead: ComponentType<{ chapterId: number; storyId: number }> = (props) => {
	const [getChaptersRead, { mutate }] = useChaptersRead(props.storyId);

	const isRead = () => !getChaptersRead.loading && getChaptersRead().has(props.chapterId);

	const markRead = async () => {
		const client = isRead() ? DeleteApiChaptersread : PostApiChaptersread;
		const res = await client({ story: props.storyId, chapter: props.chapterId });
		if (!res.ok) return;

		mutate(new Set(res.data));
	};

	return (
		<button
			type="button"
			class="read-status"
			classList={{ active: isRead() }}
			aria-label="Chapter read status"
			title={isRead() ? "Mark as unread" : "Mark as read"}
			onClick={markRead}
		>
			{isRead() ? <LucideEye /> : <LucideEyeClosed />}
		</button>
	);
};

component(
	"o-read",
	{
		chapterId: 0,
		storyId: 0,
	},
	ChapterRead,
	css,
);
