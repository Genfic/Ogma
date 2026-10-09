// markdown.test.ts
import { describe, expect, it } from "bun:test";
import { renderMarkdown } from "@h/markdown";

/** Parse rendered output so assertions see real elements, not escaped text. */
const parse = (md: string) => {
	const host = document.createElement("div");
	host.innerHTML = renderMarkdown(md);
	return host;
};

/** Every element that must never survive into the output. */
const FORBIDDEN_TAGS = ["script", "iframe", "object", "embed", "svg", "base", "form", "input", "style", "link", "meta"];

const SAFE_SCHEMES = ["http:", "https:", "mailto:"];

/** Collects concrete XSS evidence from a rendered fragment. */
const findLivePayloads = (host: HTMLElement): string[] => {
	const found: string[] = [];

	for (const tag of FORBIDDEN_TAGS) {
		if (host.querySelector(tag)) {
			found.push(`<${tag}> element survived`);
		}
	}

	for (const el of host.querySelectorAll("*")) {
		for (const attr of el.attributes) {
			const name = attr.name.toLowerCase();
			if (name.startsWith("on")) {
				found.push(`event handler ${name} on <${el.tagName.toLowerCase()}>`);
			}
			if (name === "style") {
				found.push(`inline style on <${el.tagName.toLowerCase()}>`);
			}
			if (name === "href" || name === "src") {
				const value = attr.value.trim();
				// A scheme is only present when the value has one; relative URLs are fine.
				const scheme = /^[a-z][a-z0-9+.-]*:/i.exec(value)?.[0].toLowerCase();
				if (scheme && !SAFE_SCHEMES.includes(scheme)) {
					found.push(`${scheme} in ${name} on <${el.tagName.toLowerCase()}>`);
				}
			}
		}
	}

	return found;
};

describe("renderMarkdown sanitization", () => {
	const attacks: [string, string][] = [
		["raw image onerror", `<img src=x onerror=alert(1)>`],
		["raw script tag", `<script>alert(1)</script>`],
		["raw iframe", `<iframe src="https://evil.test"></iframe>`],
		["raw svg onload", `<svg onload=alert(1)>`],
		["raw base tag", `<base href="https://evil.test">`],
		["overlay via inline style", `<img src=x style="position:fixed;inset:0;width:100vw;height:100vh">`],
		["javascript link", `[click](javascript:alert(1))`],
		["mixed-case javascript link", `[click](JaVaScRiPt:alert(1))`],
		["data uri link", `[click](data:text/html;base64,PHNjcmlwdD5hbGVydCgxKTwvc2NyaXB0Pg==)`],
		["javascript image", `![alt](javascript:alert(1))`],
		["javascript autolink", `<javascript:alert(1)>`],
		// The original finding: the heading renderer used token.raw verbatim.
		["heading image onerror", `# <img src=x onerror=alert(1)>`],
		["heading script tag", `# <script>alert(1)</script>`],
		["heading anchor clobber", `# hi <a id="body"></a>`],
		["heading javascript link", `# [x](javascript:alert(1))`],
		["heading formatted with payload", `# **bold** <img src=x onerror=alert(1)>`],
	];

	it.each(attacks)("neutralizes %s", (_name, input) => {
		expect(findLivePayloads(parse(input))).toEqual([]);
	});

	it("escapes raw HTML rather than dropping it, so text stays readable", () => {
		const host = parse(`<img src=x onerror=alert(1)>`);
		expect(host.querySelector("img")).toBeNull();
		expect(host.textContent).toContain("<img src=x onerror=alert(1)>");
	});
});

describe("renderMarkdown output is real HTML, not escaped text", () => {
	it("renders bold and italic as elements", () => {
		const host = parse(`**bold** and *italic*`);
		expect(host.querySelector("strong")?.textContent).toBe("bold");
		expect(host.querySelector("em")?.textContent).toBe("italic");
	});

	it("renders a heading as a span, matching DisableHeadings server-side", () => {
		const host = parse(`# Title`);
		expect(host.querySelector("h1")).toBeNull();
		expect(host.querySelector("span")?.textContent).toBe("Title");
	});

	it("renders inline formatting inside a heading without leaking the # marker", () => {
		const host = parse(`# **bold** heading`);
		const span = host.querySelector("span");
		expect(span?.textContent).toBe("bold heading");
		expect(span?.querySelector("strong")?.textContent).toBe("bold");
		expect(span?.innerHTML).not.toContain("#");
		expect(span?.innerHTML).not.toContain("**");
	});

	it("keeps the strikethrough, insert and mark extensions", () => {
		expect(parse(`~~gone~~`).querySelector("del")?.textContent).toBe("gone");
		expect(parse(`++ins++`).querySelector("ins")?.textContent).toBe("ins");
		expect(parse(`==marked==`).querySelector("mark")?.textContent).toBe("marked");
	});

	it("keeps subscript", () => {
		expect(parse(`H~2~O`).querySelector("sub")?.textContent).toBe("2");
	});

	it("keeps spoiler, mention and hashtag classes", () => {
		expect(parse(`||secret||`).querySelector("span.spoiler")?.textContent).toBe("secret");
		expect(parse(`@someone`).querySelector("a.mention")?.getAttribute("href")).toBe("/user/someone");
		expect(parse(`#tag`).querySelector("a.hashtag")?.getAttribute("href")).toBe("/tag/tag");
	});

	it("keeps tables, code, quotes and lists", () => {
		expect(parse(`| a | b |\n|---|---|\n| 1 | 2 |`).querySelector("table")).not.toBeNull();
		expect(parse("```js\nconst a = 1;\n```").querySelector("pre code")).not.toBeNull();
		expect(parse("`const a = 1;`").querySelector("code")?.textContent).toBe("const a = 1;");
		expect(parse(`> quoted`).querySelector("blockquote")).not.toBeNull();
		expect(parse(`- one\n- two`).querySelector("ul li")).not.toBeNull();
		expect(parse(`1. one\n2. two`).querySelector("ol li")).not.toBeNull();
		expect(parse(`---`).querySelector("hr")).not.toBeNull();
	});

	it("keeps safe link and image attributes", () => {
		const link = parse(`[site](https://example.com)`).querySelector("a");
		expect(link?.getAttribute("href")).toBe("https://example.com");

		const mail = parse(`[mail](mailto:a@b.c)`).querySelector("a");
		expect(mail?.getAttribute("href")).toBe("mailto:a@b.c");

		const relative = parse(`[rel](/user/someone)`).querySelector("a");
		expect(relative?.getAttribute("href")).toBe("/user/someone");

		const img = parse(`![alt text](https://x.com/a.png)`).querySelector("img");
		expect(img?.getAttribute("src")).toBe("https://x.com/a.png");
		expect(img?.getAttribute("alt")).toBe("alt text");
	});

	it("returns empty output for empty input", () => {
		expect(renderMarkdown("")).toBe("");
	});
});
