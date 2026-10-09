import { diff } from "@angius/htmldiff";
import { GetApiDocumentsVersion } from "@g/paths-public";
import { component } from "@h/web-components";
import { type ComponentType, noShadowDOM } from "solid-element";
import { Show, createSignal } from "solid-js";
import { Dialog, type DialogApi } from "./common/_dialog";
import css from "./doc-diff.css";

type Props = {
	slug: string;
	currentVersion: number;
	targetVersion: number;
	bodySelector?: string;
};

const DocDiff: ComponentType<Props> = (props) => {
	noShadowDOM();

	let dialogRef: DialogApi | undefined;
	const [targetVersion, setTargetVersion] = createSignal<number | null>(null);
	const [loading, setLoading] = createSignal(false);
	const [error, setError] = createSignal<string | null>(null);
	const [result, setResult] = createSignal<string | undefined>(undefined);

	const id = () => `doc-diff-${props.slug}-${props.currentVersion}-${props.targetVersion}`;

	const close = () => {
		if (result()) {
			setResult(undefined);
		}
		setTargetVersion(null);
		setError(null);
	};

	const getCurrentBody = (): string | null => {
		const sel = props.bodySelector ?? "#doc-body .md";
		const el = document.querySelector(sel);
		if (el) {
			return el.innerHTML;
		}
		const fallback = document.querySelector("#doc-body");
		if (fallback) {
			return fallback.innerHTML;
		}
		return null;
	};

	const openDiff = async () => {
		setTargetVersion(props.targetVersion);
		setLoading(true);
		setError(null);

		if (result()) {
			setResult(undefined);
		}

		try {
			const [otherRes, currentBody] = await Promise.all([
				GetApiDocumentsVersion(props.slug, props.targetVersion),
				Promise.resolve(getCurrentBody()),
			]);

			if (!otherRes.ok) {
				setError("Failed to load version content.");
				setLoading(false);
				return;
			}
			const otherBody = otherRes.data.compiledBody;
			if (!currentBody) {
				setError("Could not find current document body.");
				setLoading(false);
				return;
			}

			const targetVer = props.targetVersion;
			const currentVer = props.currentVersion;
			const oldHtml = targetVer <= currentVer ? otherBody : currentBody;
			const newHtml = targetVer <= currentVer ? currentBody : otherBody;

			setResult(diff(oldHtml, newHtml) ?? undefined);

			dialogRef?.open();
		} catch (e) {
			console.error(e);
			setError("Failed to generate diff.");
		} finally {
			setLoading(false);
		}
	};

	return (
		<>
			<button class="btn inline diff-btn" title="Show diff" onclick={() => openDiff()} aria-controls={id()}>
				diff
			</button>

			<Dialog
				id={id()}
				ref={(api) => (dialogRef = api)}
				classes={["doc-diff"]}
				contentClass="md doc-diff-content"
				header={
					<strong>
						Diff
						<Show when={targetVersion() !== null}>
							: v{targetVersion()} vs current (v{props.currentVersion})
						</Show>
					</strong>
				}
				onClose={close}
			>
				<Show when={loading()}>
					<div>Loading diff...</div>
				</Show>
				<Show when={!loading() && error()}>
					<div>{error()}</div>
				</Show>
				<Show when={!loading() && result()}>
					<div class="diff-view" innerHTML={result()} />
				</Show>
			</Dialog>
		</>
	);
};

component("doc-diff", { slug: "", currentVersion: 0, targetVersion: 0, bodySelector: ".md" }, DocDiff, [css]);
