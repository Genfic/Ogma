import { $queryAll } from "@h/dom";

const tabViews = $queryAll("[tab-view]");

for (const tabView of tabViews) {
	const tabs = tabView.querySelectorAll('[role="tab"]');
	const tabPanels = tabView.querySelectorAll<HTMLElement>('[role="tabpanel"]');

	const selectTab = (selected: Element) => {
		for (const tab of tabs) {
			tab.setAttribute("aria-selected", String(tab === selected));
		}
		for (const panel of tabPanels) {
			const isActive = panel.getAttribute("aria-labelledby") === selected.id;
			if (isActive) {
				panel.removeAttribute("hidden");
			} else {
				panel.setAttribute("hidden", "until-found");
			}
		}
	};

	for (const tab of tabs) {
		const panel = tabPanels.values().find((p) => p.getAttribute("aria-labelledby") === tab.id);
		if (panel) {
			tab.addEventListener("click", () => {
				selectTab(tab);
			});
		}
	}

	for (const panel of tabPanels) {
		// Sync selection when find-in-page reveals a hidden panel
		panel.addEventListener("beforematch", () => {
			const tab = tabs.values().find((t) => t.id === panel.getAttribute("aria-labelledby"));
			if (tab) {
				selectTab(tab);
			}
		});

		// Upgrade the server-rendered `hidden` to `hidden="until-found"` so find-in-page can
		// still reveal inactive panels; unsupported browsers degrade to plain `hidden`
		// Safari's lagging behind with support, of course
		if (panel.hidden) {
			panel.setAttribute("hidden", "until-found");
		}
	}
}
