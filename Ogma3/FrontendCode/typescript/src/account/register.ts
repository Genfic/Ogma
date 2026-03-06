import { getCookieValue } from "@h/cookies";
import { $query, $queryAll } from "@h/dom";
import { pow } from "@h/pow";

const classname = "visible";
const formInputs = $queryAll("input[id]");

const powData = $query("[data-pow-token][data-pow-diff]");
const token = powData.dataset.powToken;
const diff = powData.dataset.powDiff;

if (!token || !diff) throw new Error("Missing pow data");

let powStarted = false;
const runPow = async () => {
	const res = await pow(token, Number.parseInt(diff, 10));
	console.log("PoW", res);
	for (const [k, v] of Object.entries({ ...res, token })) {
		const el = $query<HTMLInputElement>(`[id="Input_Pow${k}" i]`);
		el.value = String(v);
	}

	$query<HTMLButtonElement>('button[type="submit"]').disabled = false;
};

for (const input of formInputs) {
	const info = $query(`[data-for="${input.id}"]`, true);
	if (!info) continue;

	input.addEventListener("focusin", async () => {
		info.classList.add(classname);
		if (!powStarted) {
			powStarted = true;
			await runPow();
		}
	});
	input.addEventListener("focusout", () => {
		info.classList.remove(classname);
	});
}

const turnstile = $query(".cf-turnstile");
const theme = getCookieValue("theme");
turnstile.dataset.theme = theme;
