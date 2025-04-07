import { parseDom } from "@h/dom";
import { minifyHtml } from "@h/minify.macro" with { type: "macro" };

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

const tpl = minifyHtml(`
	<nav class="button-group toolbar">
	  <button type="button" class="btn" data-action="${Action.bold}" title="${Action.bold}">
		<o-icon icon="lucide:bold" class="material-icons-outlined"></o-icon>
	  </button>
	  <button type="button" class="btn" data-action="${Action.italic}" title="${Action.italic}">
		<o-icon icon="lucide:italic" class="material-icons-outlined" ></o-icon>
	  </button>
	  <button type="button" class="btn" data-action="${Action.underline}" title="${Action.underline}">
		<o-icon icon="lucide:underline" class="material-icons-outlined" ></o-icon>
	  </button>
	  <button type="button" class="btn" data-action="${Action.spoiler}" title="${Action.spoiler}">
		<o-icon icon="lucide:eye-closed" class="material-icons-outlined" ></o-icon>
	  </button>
	  <button type="button" class="btn" data-action="${Action.link}" title="${Action.link}">
		<o-icon icon="lucide:link" class="material-icons-outlined" ></o-icon>
	  </button>
	</nav>`);

const map: Record<Action, PrefixSuffix> = {
	[Action.bold]: { prefix: "**", suffix: "**" },
	[Action.italic]: { prefix: "*", suffix: "*" },
	[Action.underline]: { prefix: "_", suffix: "_" },
	[Action.spoiler]: { prefix: "||", suffix: "||" },
	[Action.link]: { prefix: "[", suffix: "]()" },
};

const areas = [...document.querySelectorAll("[data-md=true]")] as (HTMLTextAreaElement | HTMLInputElement)[];

for (const area of areas) {
	const vDom = parseDom(tpl);

	for (const btn of [...vDom.querySelectorAll("button.btn[data-action]")] as HTMLElement[]) {
		const action: Action = btn.dataset.action as Action;

		btn.addEventListener("click", () => {
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
