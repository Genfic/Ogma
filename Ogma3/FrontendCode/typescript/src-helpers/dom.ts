const parser = new DOMParser();

export const parseDom = (html: string): HTMLElement => {
	return parser.parseFromString(html, "text/html").body.firstElementChild as HTMLElement;
};
