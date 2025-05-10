import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { useChaptersRead } from "./common/_chaptersReadData";
import { PostApiChaptersread } from "@g/paths-public";

const ChapterRead: ComponentType<{ chapterId: number; storyId: number }> = (props) => {
	noShadowDOM();

	const [getChaptersRead, _] = useChaptersRead(props.storyId);

	const isRead = () => getChaptersRead().some((c) => c === props.chapterId);

	const markRead = async () => {
		const res = await PostApiChaptersread({ story: props.storyId, chapter: props.chapterId });
		if (!res.ok) return;
	};

	return (
		<button type="button" class="read-status" aria-label="Chapter read status" onClick={markRead}>
			<o-icon icon={isRead() ? "lucide:eye-on" : "lucide:eye-off"} />
		</button>
	);
};

customElement(
	"o-read",
	{
		chapterId: 0,
		storyId: 0,
	},
	ChapterRead,
);
