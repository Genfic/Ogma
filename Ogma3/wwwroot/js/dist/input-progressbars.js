(_ => {
    var _a, _b;
    // This monstrosity grabs all `input` and `textarea` tags and puts them inside a single array
    // so that it's easier to use/ Because of course `querySelectorAll()` returns some weird shit instead
    // of a proper array. Thank fuck for the spread operator.
    const inputs = [
        ...[...document.querySelectorAll('input.o-form-control')],
        ...[...document.querySelectorAll('textarea.o-form-control')]
    ];
    for (const i of inputs) {
        // If there's no count specified, get max length. If that's not there, just use 0.
        let max = i.dataset.maxCount
            ? Number(i.dataset.maxCount)
            : (_a = i.maxLength) !== null && _a !== void 0 ? _a : 0;
        let min = (_b = Number(i.dataset.valLengthMin)) !== null && _b !== void 0 ? _b : 0;
        // Create the main container
        let counter = document.createElement('div');
        counter.classList.add('counter');
        // Create the progress bar proper
        let progress = document.createElement('div');
        progress.classList.add('o-progress-bar');
        // Create the character counter
        let count = document.createElement('span');
        // If `data-max-count` has been specified, that means we're counting elements of a comma-separated list
        // If not, we're just counting chars
        let length = i.dataset.maxCount
            ? i.value.split(',').length
            : i.value.length;
        count.innerText = `${length}/${max}`;
        // Append the progress bar to the container
        counter.appendChild(progress);
        // If the `data-wordcount` property is there, create a wordcount element and append it
        let wordcount;
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
        i.addEventListener('input', _ => {
            var _a;
            // If `data-max-count` has been specified, that means we're counting elements of a comma-separated list
            // If not, we're just counting chars
            let length = i.dataset.maxCount
                ? i.value.split(',').length
                : i.value.length;
            // Update character counter
            count.innerText = `${length}/${max}`;
            // Update progress bar progress
            progress.style.width = `${Math.min(100, 100 * (length / max))}%`;
            // If `data-wordcount` has been specified, update that as well
            if (i.dataset.wordcount) {
                wordcount.innerText = ((_a = i.value.properSplit(/\s+/)) !== null && _a !== void 0 ? _a : []).length.toString() + ' words';
            }
            // Check if the input is valid
            counter.classList.toggle('invalid', length < min || length > max);
        });
    }
})();
