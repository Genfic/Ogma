import { $queryAll } from "@h/dom";

const tabViews = $queryAll("[tab-view]");

for (const tabView of tabViews) {
	const tabs = [...tabView.querySelectorAll('[role="tab"]')];
	const tabPanels = [...tabView.querySelectorAll('[role="tabpanel"]')];

	for (const tab of tabs) {
		const panel = tabPanels.find((p) => p.getAttribute("aria-labelledby") === tab.id);
		if (panel) {
			tab.addEventListener("click", () => {
				for (const t of tabs) {
					t.classList.remove("active");
				}
				for (const p of tabPanels) {
					p.classList.remove("active");
				}
				tab.classList.add("active");
				panel.classList.add("active");
			});
		}
	}
}
