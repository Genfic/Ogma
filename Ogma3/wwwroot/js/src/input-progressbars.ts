// eslint-disable-next-line @typescript-eslint/no-unused-vars,no-redeclare
interface String {
    properSplit(split: string|RegExp) : Array<string>
}

(() => {
	// This monstrosity grabs all `input` and `textarea` tags and puts them inside a single array
	// so that it's easier to use/ Because of course `querySelectorAll()` returns some weird shit instead
	// of a proper array. Thank fuck for the spread operator.
	const inputs: (HTMLInputElement|HTMLTextAreaElement)[] = [
		...[...document.querySelectorAll('input.o-form-control:not([disabled])')], 
		...[...document.querySelectorAll('textarea.o-form-control:not([disabled])')]
	] as (HTMLInputElement|HTMLTextAreaElement)[];
    
    
	for (const i of inputs) {
		console.log(`Attaching ${i.type}`);
        
		// If there's no count specified, get max length. If that's not there, just use 0.
		let max: number;
		if (i.dataset.maxCount || i.dataset.valLengthMax) {
			// `data-max-count` is set, so that's what we use
			max = Number(i.dataset.maxCount ?? i.dataset.valLengthMax);
		} else if(i.dataset.valFilesizeMax && i.type === 'file') {
			// It's a file input with maximum file size defined, so we get that
			max = Number(i.dataset.valFilesizeMax);
		} else {
			// Just a regular input, so grab `maxLength`
			max = i.maxLength ?? 0;
		}
        
		// Function to get the current size
		const currentSize: () => number = function (): number {
			if (i.dataset.maxCount) {
				// `data-max-count` is set, so we're counting comma-separated values
				return i.value.split(',').length;
			} else if(i.dataset.valFilesizeMax && i.type === 'file') {
				// It's a file input with maximum file size defined, so we get the file size
				return (i as HTMLInputElement).files[0]?.size ?? 0;
			} else {
				// Just a regular ol' input, get the value length
				return i.value.length;
			}
		};
        
		const min: number = Number(i.dataset.valLengthMin) ?? 0;
        
		// Create the main container
		const counter: HTMLElement = document.createElement('div');
		counter.classList.add('counter');
        
		// Create the progress bar proper
		const progress: HTMLElement = document.createElement('div');
		progress.classList.add('o-progress-bar');
        
		// Create the character counter
		const count: HTMLElement = document.createElement('span');
		const length = currentSize();
		count.innerText = `${length}/${max}`;
        
		// Append the progress bar to the container
		counter.appendChild(progress);
        
		// If the `data-wordcount` property is there, create a wordcount element and append it
		let wordcount: HTMLElement;
		if (i.dataset.wordcount) {
			wordcount = document.createElement('span');
			wordcount.innerText = i.value.properSplit(/\s+/).length.toString() + ' words';
			counter.appendChild(wordcount);
		}
        
		// Append character counter
		counter.appendChild(count);
        
		// Append the whole thing right after the target input element
		i.after(counter); 
        
		// Listen to input
		console.log(`Listening to ${i.type}`);
		i.addEventListener('input', () => {
			
			const length = currentSize();
			// Update character counter
			count.innerText = `${length}/${max}`;
            
			// Update progress bar progress
			progress.style.width = `${Math.min(100, 100 * (length / max))}%`;

			// If `data-wordcount` has been specified, update that as well
			if (i.dataset.wordcount) {
				wordcount.innerText = (i.value.properSplit(/\s+/) ?? []).length.toString() + ' words';
			}
            
			// Check if the input is valid
			counter.classList.toggle('invalid', length < min || length > max);
		});
        
	}
})();