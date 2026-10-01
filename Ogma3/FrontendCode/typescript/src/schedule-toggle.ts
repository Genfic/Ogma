import { $queryAll } from "@h/dom";

for (const publish of $queryAll<HTMLInputElement>("[name='Input.Publish']")) {
	const schedule = document.querySelector<HTMLInputElement>("[name='Input.Schedule']");

	if (!schedule) {
		continue;
	}

	const sync = () => {
		schedule.disabled = publish.checked;
	};

	sync();

	publish.addEventListener("change", sync);
}