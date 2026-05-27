import { trim } from "@h/string-helpers";

const areas = document.querySelectorAll("textarea[allow-paste]");
const csrf = (document.querySelector("input[name=__RequestVerificationToken]") as HTMLInputElement).value;

for (const area of areas) {
	watchForPaste(area as HTMLTextAreaElement);
}

function watchForPaste(area: HTMLTextAreaElement) {
	area.addEventListener("paste", async (e) => {
		const items = e.clipboardData?.items;
		if (!items) return;

		const file = [...items].find((item) => item.kind === "file" && item.type.startsWith("image/"))?.getAsFile();
		if (!file) return;

		e.preventDefault();

		const start = area.selectionStart;
		const end = area.selectionEnd;
		const originalValue = area.value;

		const placeholder = `![Uploading ${file.name}...]()`;

		area.value = originalValue.slice(0, start) + placeholder + originalValue.slice(end);
		const offset = start + placeholder.length;
		area.setSelectionRange(offset, offset);

		try {
			const url = await uploadImage(file);
			const md = `![${file.name}](${url})`;

			area.value = area.value.replace(placeholder, md);

			const newCursorPos = area.value.indexOf(md) + md.length;
			area.setSelectionRange(newCursorPos, newCursorPos);
		} catch (err) {
			console.error("Error uploading image:", err);
			const failureMsg = `![Error uploading ${file.name}]()`;
			area.value = area.value.replace(placeholder, failureMsg);
		}
	});
}

async function uploadImage(file: File) {
	const formData = new FormData();
	formData.append("file", file, file.name);

	const res = await fetch("/api/files/upload", {
		method: "POST",
		body: formData,
		headers: {
			RequestVerificationToken: csrf,
		},
	});

	if (!res.ok) {
		throw new Error(`API responded with status ${res.status}.`);
	}

	const url = await res.text();
	return trim(url, '"');
}
