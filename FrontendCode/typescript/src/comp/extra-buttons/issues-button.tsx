import { analyze, applyIssues, type Issue, RULES } from "@h/md-issues";
import { component } from "@h/web-components";
import LucidePencil from "icon:lucide:pencil";
import LucideRefreshCw from "icon:lucide:refresh-cw";
import { type ComponentType, noShadowDOM } from "solid-element";
import { For, Show } from "solid-js";
import { Dialog, type DialogApi } from "../common/_dialog";
import type { ExtraButtonContext } from "./extra-button-types";
import extra from "./extra-button.css";
import css from "./issues-button.css";

/** Characters of context shown either side of an issue. */
const CONTEXT = 28;

const IssueSnippet: Component<{ source: string; issue: Issue }> = (props) => {
	const from = () => Math.max(0, props.issue.start - CONTEXT);
	const to = () => Math.min(props.source.length, props.issue.end + CONTEXT);

	return (
		<code class="issues-snippet">
			{from() > 0 ? "…" : ""}
			{props.source.slice(from(), props.issue.start)}
			<span class="hl">{props.source.slice(props.issue.start, props.issue.end)}</span>
			{props.source.slice(props.issue.end, to())}
			{to() < props.source.length ? "…" : ""}
		</code>
	);
};

const IssuesButton: ComponentType<{ context?: ExtraButtonContext }> = (props) => {
	noShadowDOM();

	let dialog = $signal<DialogApi>();
	let text = $signal("");
	let issues = $signal<Issue[]>([]);

	const grouped = $memo(
		RULES.map((rule) => ({ rule, issues: $get(issues)().filter((issue) => issue.rule === rule.id) })).filter(
			(group) => group.issues.length > 0,
		),
	);

	/** Scan the editor, reporting whether it was reachable at all. */
	const scan = () => {
		const source = props.context?.input.value;
		if (source === undefined) {
			return false;
		}

		text = source;
		issues = analyze(source);

		return true;
	};

	/** Write `value` back, telling the editor and anything listening for input. */
	const write = (value: string) => {
		const context = props.context;
		if (!context) {
			return;
		}

		context.input.value = value;
		context.finishEdit(value.length);
		context.input.dispatchEvent(new Event("input", { bubbles: true }));
	};

	const show = () => {
		if (scan()) {
			dialog?.open();
		}
	};

	const fix = (predicate: (issue: Issue) => boolean) => {
		const source = $get(text)();
		const next = applyIssues(source, $get(issues)().filter(predicate)).text;

		if (next === source) {
			return;
		}

		write(next);
		scan();
	};

	return (
		<>
			<button type="button" class="btn action-btn" title="Writing issues" onClick={show}>
				<LucidePencil />
			</button>

			<Dialog ref={(api) => (dialog = api)} header={<h2>Writing issues (beta)</h2>} contentClass="issues">
				<Show
					when={$get(issues)().length > 0}
					fallback={<p class="issues-clean">Nothing to fix in this text.</p>}
				>
					<p class="issues-count">
						{$get(issues)().length} issue{$get(issues)().length === 1 ? "" : "s"} found
					</p>

					<button type="button" class="btn issues-fix-all" onClick={() => fix(() => true)}>
						<LucideRefreshCw />
						Fix all
					</button>

					<For each={$get(grouped)()}>
						{(group) => (
							<section class="issues-rule">
								<header>
									<h3>{group.rule.title}</h3>
									<button
										type="button"
										class="btn issues-fix-rule"
										onClick={() => fix((issue) => issue.rule === group.rule.id)}
									>
										Fix all ({group.issues.length})
									</button>
								</header>

								<p class="issues-rule-description">{group.rule.description}</p>

								<For each={group.issues}>
									{(issue) => (
										<div class="issues-item">
											<p class="issues-message">{issue.message}</p>
											<IssueSnippet source={text} issue={issue} />
											<button
												type="button"
												class="btn issues-fix-one"
												title="Fix this issue"
												onClick={() => fix((candidate) => candidate === issue)}
											>
												Fix
											</button>
										</div>
									)}
								</For>
							</section>
						)}
					</For>
				</Show>
			</Dialog>
		</>
	);
};

component("issues-btn", { context: undefined }, IssuesButton, [extra, css], ["context"]);
