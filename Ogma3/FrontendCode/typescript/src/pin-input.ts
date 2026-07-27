import { $queryAll } from "@h/dom";

const initPinInput = (container: HTMLElement): void => {
	const boxes = [...container.querySelectorAll<HTMLInputElement>("input.pin-box")];
	const hidden = container.querySelector<HTMLInputElement>("input.pin-value");
	const maxDigits = Number(container.dataset.maxDigits);

	const digits: string[] = Array.from<string>({ length: maxDigits }).fill("");
	const maskTimers: ReturnType<typeof setTimeout>[] = [];
	let revealed = false;

	const syncHidden = (): void => {
		if (hidden) {
			hidden.value = digits.join("");
		}
	};

	const displayBox = (el: HTMLInputElement, i: number, char: string): void => {
		el.value = char;
		el.classList.add("filled");

		if (revealed) {
			return;
		}

		clearTimeout(maskTimers[i]);
		maskTimers[i] = setTimeout(() => {
			if (digits[i] === char) {
				el.value = "●";
				el.classList.add("masked");
			}
		}, 350);
	};

	const onKeyDown = (e: KeyboardEvent): void => {
		console.log("KeyDown", e.key);

		if (!(e.target instanceof HTMLInputElement)) {
			return;
		}
		const el = e.target;
		const i = Number(el.dataset.index);

		if (e.key === "Backspace") {
			if (el.value === "" && i > 0) {
				e.preventDefault();
				boxes[i - 1].focus();
				boxes[i - 1].value = "";
				digits[i - 1] = "";
				boxes[i - 1].classList.remove("filled");
			} else {
				digits[i] = "";
				el.classList.remove("filled");
			}
			syncHidden();
		} else if (e.key === "ArrowLeft" && i > 0) {
			e.preventDefault();
			boxes[i - 1].focus();
		} else if (e.key === "ArrowRight" && i < maxDigits - 1) {
			e.preventDefault();
			boxes[i + 1].focus();
		}
	};

	const onInput = (e: InputEvent): void => {
		console.log("Input");

		if (!(e.target instanceof HTMLInputElement)) {
			return;
		}
		const el = e.target;
		const i = Number(el.dataset.index);
		const raw = el.value.replace(/[^0-9]/g, "");
		const char = raw.slice(-1);

		el.classList.remove("masked");

		console.log(char);

		if (!char) {
			el.value = "";
			digits[i] = "";
			syncHidden();
			return;
		}

		digits[i] = char;
		displayBox(el, i, char);
		syncHidden();

		if (i < maxDigits - 1) {
			boxes[i + 1].focus();
		}
	};

	for (const box of boxes) {
		box.addEventListener("keydown", onKeyDown);
		box.addEventListener("input", onInput);
		box.addEventListener("focus", () => box.select());
	}
};

for (const input of $queryAll(".pin-input")) {
	initPinInput(input);
}
