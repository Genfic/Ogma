import { Blogpost } from "@g/ctconfig";
import { component } from "@h/web-components";
import LucideSeparator from "icon:lucide:separator-horizontal";
import type { ComponentType } from "solid-element";
import type { ExtraButtonContext } from "./extra-button-types";
import shared from "../shared.css";
import css from "./extra-button.css";

const ReadMoreButton: ComponentType<{ context?: ExtraButtonContext }> = (props) => {
	const insert = () => {
		const area = props.context?.input;
		if (!area) return;

		const val = area.value;
		let start = area.selectionStart ?? 0;
		let end = area.selectionEnd ?? 0;
		const e = end;

		while (start > 0 && val[start - 1] === "\n") {
			start--;
		}

		while (end < val.length && val[end] === "\n") {
			end++;
		}

		const text = `\n\n${Blogpost.CutoffMarker}\n\n`;

		area.setRangeText(text, start, end);
		props.context?.finishEdit(e + text.length, e + text.length);
	};

	return (
		<button type="button" class="btn action-btn" title="Read More" onClick={insert}>
			<LucideSeparator />
		</button>
	);
};

component("read-more-btn", { context: undefined }, ReadMoreButton, [shared, css], ["context"]);
