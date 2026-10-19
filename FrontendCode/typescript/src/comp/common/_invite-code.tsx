import type { InviteCodeDto } from "@g/types-public";
import { toCurrentTimezone } from "@h/date-helpers";
import { long } from "@h/tinytime-templates";
import LucideClipboardCopy from "icon:lucide:clipboard-copy";
import LucideLink from "icon:lucide:link";
import LucideQrCode from "icon:lucide:qr-code";
import { createSignal, Show, JSXElement } from "solid-js";

const date = (dt: string | Date | null) => dt && long.render(toCurrentTimezone(new Date(dt)));

const copyString = (str: string) => {
	navigator.clipboard.writeText(str).then(
		() => alert("Copied"),
		(e) => {
			alert("Could not copy");
			console.error(e);
		},
	);
};

interface InviteCodesProps {
	code: InviteCodeDto;
	actions?: JSXElement;
	class?: string | undefined | null | false;
}

if (!customElements.get("qr-code")) {
	await import("../qr-code.js");
}

const size = () => Math.min(window.innerWidth, 250);

export const InviteCode = (props: InviteCodesProps) => {
	const code = () => props.code;
	const url = () => `${window.location.origin}/register?invite=${code().code}`;

	const [showQr, setShowQr] = createSignal(false);

	return (
		<li class={props.class ? props.class : undefined}>
			<div class="deco" style={{ background: code().usedByUserName ? "green" : "gray" }} />
			<div class="main">
				<h3 class="name">
					<span class="monospace">{code().code}</span>
				</h3>

				<Show when={showQr()}>
					<qr-code prop:data={url()} prop:height={size()} prop:width={size()} />
				</Show>

				<span class="desc">
					Issued by <strong>{code().issuedByUserName ?? code().issuedByType}</strong> on{" "}
					<strong>{date(code().issueDate)}</strong>
				</span>

				{code().usedByUserName && code().usedDate ? (
					<span class="desc">
						Redeemed by <strong>{code().usedByUserName}</strong> on <strong>{date(code().usedDate)}</strong>
					</span>
				) : null}
			</div>
			<div class="actions">
				<button
					type="button"
					class="action"
					title={code().usedByUserName ? "Already used" : "Copy invite code"}
					onClick={[copyString, code().code]}
					disabled={!!code().usedByUserName}
				>
					<LucideClipboardCopy />
				</button>
				<button
					type="button"
					class="action"
					title={code().usedByUserName ? "Already used" : "Copy registration link"}
					onClick={[copyString, url()]}
					disabled={!!code().usedByUserName}
				>
					<LucideLink />
				</button>
				<button type="button" class="action" title="Show QR code" onClick={() => setShowQr(!showQr())}>
					<LucideQrCode />
				</button>
				{props.actions}
			</div>
		</li>
	);
};
