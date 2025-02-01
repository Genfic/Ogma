import { parseDom } from "../../src-helpers/dom";

enum Action {
	bold = "bold",
	italic = "italic",
	underline = "underline",
	spoiler = "spoiler",
	link = "link",
}

interface PrefixSuffix {
	prefix: string;
	suffix: string;
}

const tpl = `
		<nav class="button-group toolbar">
		  <button type="button" class="btn" data-action="${Action.bold}" title="${Action.bold}">
		    <span class="material-icons-outlined">format_bold</span>
		  </button>
		  <button type="button" class="btn" data-action="${Action.italic}" title="${Action.italic}">
		    <span class="material-icons-outlined">format_italic</span>
		  </button>
		  <button type="button" class="btn" data-action="${Action.underline}" title="${Action.underline}">
		    <span class="material-icons-outlined">format_underlined</span>
		  </button>
		  <button type="button" class="btn" data-action="${Action.spoiler}" title="${Action.spoiler}">
		    <span class="material-icons-outlined">visibility</span>
		  </button>
		  <button type="button" class="btn" data-action="${Action.link}" title="${Action.link}">
		    <span class="material-icons-outlined">link</span>
		  </button>
		</nav>`
	.split("\n")
	.map((l) => l.trim())
	.join("");

const map = new Map<Action, PrefixSuffix>([
	[Action.bold, { prefix: "**", suffix: "**" }],
	[Action.italic, { prefix: "*", suffix: "*" }],
	[Action.underline, { prefix: "_", suffix: "_" }],
	[Action.spoiler, { prefix: "||", suffix: "||" }],
	[Action.link, { prefix: "[", suffix: "]()" }],
]);

const areas = [...document.querySelectorAll("[data-md=true]")] as (HTMLTextAreaElement | HTMLInputElement)[];

for (const area of areas) {
	const vDom = parseDom(tpl);

	for (const btn of [...vDom.querySelectorAll("button.btn[data-action]")] as HTMLElement[]) {
		const action: Action = Action[btn.dataset.action];
		btn.addEventListener("click", (_) => {
			const { prefix, suffix } = map[action];
			const start = area.selectionStart;
			const end = area.selectionEnd;

			const text = area.value.substring(start, end);

			area.setRangeText(`${prefix}${text}${suffix}`, start, end, "preserve");
			area.selectionStart = area.selectionEnd = end + prefix.length;
			area.focus();
		});
	}

	area.before(vDom);
}
