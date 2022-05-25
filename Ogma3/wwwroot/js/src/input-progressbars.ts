(() => {
	enum InputType {
		Validated,
		File,
		Regular,
	}

	// This monstrosity grabs all `input` and `textarea` tags and puts them inside a single array
	// so that it's easier to use. Because of course `querySelectorAll()` returns some weird shit instead
	// of a proper array. Thank fuck for the spread operator.
	const inputs: (HTMLInputElement | HTMLTextAreaElement)[] = [
		...[
			...document.querySelectorAll(
				"input.o-form-control:not([disabled]):not([nobar])"
			),
		],
		...[
			...document.querySelectorAll(
				"textarea.o-form-control:not([disabled]):not([nobar])"
			),
		],
	] as (HTMLInputElement | HTMLTextAreaElement)[];

	for (const input of inputs) {
		log.log(`Attaching ${input.type}`);

		let type: InputType;
		if (
			input.dataset.maxCount ||
			input.dataset.valLengthMax ||
			input.dataset.valMaxlengthMax
		) {
			// One of the validation parameters is set so it's a validated input
			type = InputType.Validated;
		} else if (input.dataset.valFilesizeMax && input.type === "file") {
			// File size validation parameter is set and the input type is file, so it's a file
			type = InputType.File;
		} else {
			// It's something else entirely
			type = InputType.Regular;
		}

		// If there's no count specified, get max length. If that's not there, just use 0.
		const max: number = {
			[InputType.Validated]: Number(
				input.dataset.maxCount ??
					input.dataset.valLengthMax ??
					input.dataset.valMaxlengthMax
			),
			[InputType.File]: Number(input.dataset.valFilesizeMax),
			[InputType.Regular]: input.maxLength ?? 0,
		}[type];

		// Sometimes we have the minimum value as well, if not let's just ensure it's not 0 or less
		const min: number = Number(input.dataset.valLengthMin) ?? 0;

		// Function to get the current size
		const currentSize: () => number = function (): number {
			if (input.dataset.maxCount) {
				// `data-max-count` is set, so we're counting comma-separated values
				return input.value.split(",").length;
			} else if (input.dataset.valFilesizeMax && input.type === "file") {
				// It's a file input with maximum file size defined, so we get the file size
				return (input as HTMLInputElement).files[0]?.size ?? 0;
			} else {
				// Just a regular ol' input, get the value length
				return input.value.length;
			}
		};

		// Create the main container
		const counter: HTMLElement = document.createElement("div");
		counter.classList.add("counter");

		// Create the progress bar proper
		const progress: HTMLElement = document.createElement("div");
		progress.classList.add("o-progress-bar");

		// Create the character counter
		const count: HTMLElement = document.createElement("span");
		const length = currentSize();
		count.innerText = `${length}/${max}${
			type === InputType.File ? " bytes" : ""
		}`;

		// Append the progress bar to the container
		counter.appendChild(progress);

		// If the `data-wordcount` property is there, create a wordcount element and append it
		let wordcount: HTMLElement;
		if (input.dataset.wordcount) {
			wordcount = document.createElement("span");
			wordcount.innerText =
				input.value.properSplit(/\s+/).length.toString() + " words";
			counter.appendChild(wordcount);
		}

		// Append character counter
		counter.appendChild(count);

		// Append the whole thing right after the target input element
		input.after(counter);

		// Listen to input
		log.log(`Listening to ${input.type}`);
		input.addEventListener("input", () => {
			const length = currentSize();
			// Update character counter
			count.innerText = `${length}/${max}${
				type === InputType.File ? " bytes" : ""
			}`;

			// Update progress bar progress
			progress.style.width = `${Math.min(100, 100 * (length / max))}%`;

			// If `data-wordcount` has been specified, update that as well
			if (input.dataset.wordcount) {
				wordcount.innerText =
					(input.value.properSplit(/\s+/) ?? []).length.toString() +
					" words";
			}

			// Check if the input is valid
			counter.classList.toggle("invalid", length < min || length > max);
		});
	}
})();