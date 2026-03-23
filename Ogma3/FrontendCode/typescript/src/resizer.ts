import { $queryAll } from "@h/dom";
import { modifyImage } from "@h/image-helper";

const element = $queryAll("[data-resize]");
for (const el of element) {
	if (!(el instanceof HTMLInputElement)) {
		continue;
	}

	const form = el.closest("form");
	const submitBtn = form?.querySelector("button[type=submit]") as HTMLButtonElement | null;

	if (!form || !submitBtn) {
		continue;
	}

	form.addEventListener("submit", (e) => {
		if (submitBtn.disabled) {
			e.preventDefault();
		}
	});

	const [width, height] = el.dataset.resize
		?.split("x")
		?.map((s) => (s.length > 0 ? Number.parseInt(s, 10) : undefined)) ?? [undefined, undefined];

	const resize = (!!width && width > 0) || (!!height && height > 0);

	el.addEventListener("change", async (e) => {
		submitBtn.disabled = true;

		const files = (e.target as HTMLInputElement).files;
		if (!files || files.length === 0) {
			submitBtn.disabled = false;
			return;
		}
		el.files = await modifyImage(files[0], { format: "image/webp", resize, width, height });

		console.log("Resized and substituted image.");

		submitBtn.disabled = false;
	});
}
