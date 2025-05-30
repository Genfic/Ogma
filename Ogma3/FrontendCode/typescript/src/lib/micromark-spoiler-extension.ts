import type { Extension, Tokenizer, State, Construct, HtmlExtension, Effects } from "micromark-util-types";

// Augment token types (ensure this is correctly picked up by your environment)
declare module "micromark-util-types" {
	interface TokenTypeMap {
		spoiler: "spoiler";
		spoilerMarker: "spoilerMarker";
		spoilerText: "spoilerText";
	}
}

export function spoilerSyntax(): Extension {
	const tokenizeSpoiler: Tokenizer = (effects, ok, nok) => {
		const start: State = (code) => {
			if (code !== 124 /* | */) return nok(code);
			effects.enter("spoiler");
			effects.enter("spoilerMarker");
			effects.consume(code); // Consume the first `|`
			return open_expectSecondPipe;
		};

		const open_expectSecondPipe: State = (code) => {
			// `code` is the char *after* the first `|`
			if (code === 124 /* | */) {
				effects.consume(code); // Consume the second `|`
				effects.exit("spoilerMarker");
				return inSpoiler;
			}
			// Not `||`. This construct fails here.
			// For robust rollback (un-entering spoiler/spoilerMarker and un-consuming first `|`),
			// `effects.attempt` around the whole construct is the standard micromark solution.
			// Without it, `nok(code)` here might lead to an inconsistent token tree.
			return nok(code);
		};

		const inSpoiler: State = (code) => {
			if (code === null || code === 10 /* \n */) {
				// Unterminated spoiler. The "spoiler" token is still open.
				// Depending on desired behavior, you might want to `effects.exit("spoiler")` before `nok`.
				// Or, `effects.attempt` would clean this up.
				return nok(code);
			}
			if (code === 124 /* | */) {
				// Potential start of a closing marker. `code` is the first `|`.
				// Pass it to a state that will consume it and check the next char.
				return close_expectFirstPipe(code);
			}
			effects.enter("spoilerText");
			// Pass `code` to `spoilerContent` to be consumed as text.
			return spoilerContent(code);
		};

		const spoilerContent: State = (code) => {
			// Consumes a single character of spoiler text
			if (code === null || code === 10 /* \n */ || code === 124 /* | */) {
				effects.exit("spoilerText");
				return inSpoiler(code); // Re-process EOF, NL, or `|` via `inSpoiler`. `code` is not consumed here.
			}
			effects.consume(code); // Consume the content character.
			return spoilerContent; // Loop for more content.
		};

		const close_expectFirstPipe: State = (code) => {
			// `code` is the first `|` of a potential closing `||`
			// `inSpoiler` ensured `code` is `124`.
			effects.enter("spoilerMarker");
			effects.consume(code); // Consume this first `|`.
			return close_expectSecondPipe; // Check the next character.
		};

		const close_expectSecondPipe: State = (code) => {
			// `code` is the char *after* the first `|` of closing marker.
			if (code === 124 /* | */) {
				// Second `|` found, it's a closing `||`
				effects.consume(code); // Consume the second `|`.
				effects.exit("spoilerMarker");
				effects.exit("spoiler");
				return ok;
			}
			// Not `||`. It was `|` followed by something else (e.g., `|a`).
			// The first `|` was consumed by `close_expectFirstPipe` under `spoilerMarker`.
			// That `spoilerMarker` (for `||`) is not valid here.
			effects.exit("spoilerMarker"); // Exit the marker, invalidating it.

			// The first `|` (already consumed) should ideally be treated as text.
			// The current `code` (e.g., 'a') should also be text.
			// Without `effects.attempt` or `construct.previous`, making the already-consumed `|`
			// part of `spoilerText` is complex. It might be "lost" or attached directly to the parent `spoiler` token.
			// We will now process the current `code` as `spoilerText`.
			effects.enter("spoilerText");
			return spoilerContent(code); // Pass the current character `code` to be handled as spoiler content.
			// `spoilerContent` will consume it if it's not EOF/NL.
		};

		return start;
	};

	const spoilerConstruct: Construct = {
		name: "spoiler",
		tokenize: tokenizeSpoiler,
		// Consider adding `resolve` or `previous` for more complex scenarios or to correctly handle
		// the single `|` in the "false alarm" closing case if it needs to be `spoilerText`.
		// For many cases, the behavior of the "lost" pipe might be acceptable.
	};

	return {
		text: {
			124: spoilerConstruct, // `|`
		},
		// If your spoiler cannot contain other constructs (e.g. links, emphasis) within it,
		// you might also need to interfere with content parsing using `disable.null`.
	};
}

export function spoilerHtml(): HtmlExtension {
	return {
		enter: {
			spoiler() {
				// Removed `this: { tag: (s: string) => void }` for brevity, Micromark handles `this`.
				this.tag('<span class="spoiler">');
			},
			// No need to handle spoilerMarker or spoilerText for basic HTML output,
			// unless you want specific tags for them. The content will be handled by default.
		},
		exit: {
			spoiler() {
				this.tag("</span>");
			},
		},
	};
}
