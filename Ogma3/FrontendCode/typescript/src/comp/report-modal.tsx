import { Report } from "@g/ctconfig";
import { PostApiReports as postReport } from "@g/paths-public";
import type { EReportableContentTypes } from "@g/types-public";
import { component } from "@h/web-components";
import { type ComponentType, noShadowDOM } from "solid-element";
import { onMount } from "solid-js";
import { Dialog, type DialogApi } from "./common/_dialog";

export type ReportModalElement = HTMLElement & {
	createNew: (id: number | string, type: EReportableContentTypes) => void;
};

const ReportModal: ComponentType<{
	openSelector?: string | undefined;
	csrf: string;
	itemId: number | string;
	itemType: EReportableContentTypes;
}> = (props, { element }) => {
	noShadowDOM();

	const rules = {
		min: Report.MinReasonLength,
		max: Report.MaxReasonLength,
	};

	let dialogRef = $signal<DialogApi>();
	let reason = $signal("");
	let success = $signal<boolean | null>(null);
	let itemId = $signal(props.itemId);
	let itemType = $signal(props.itemType);

	onMount(() => {
		if (props.openSelector) {
			const el = document.querySelector(props.openSelector);
			if (el) {
				el.addEventListener("click", show);
			} else {
				console.log(`Element for selector ${props.openSelector} not found`);
			}
		} else {
			element.createNew = createNew;
		}
	});

	const show = () => {
		dialogRef?.open();
	};

	const createNew = (id: number, type: EReportableContentTypes) => {
		itemId = id;
		itemType = type;
		dialogRef?.open();
	};

	const validate = () => reason && reason.length >= rules.min && reason.length <= rules.max;
	const chars = () => reason.length;

	const message = () => {
		switch (success) {
			case true:
				return { msg: "Report delivered!", cls: "green" };
			case false:
				return { msg: "An error has occurred. Retry?", cls: "red" };
			default:
				return { msg: "Send report", cls: null };
		}
	};

	const submit = async (e: SubmitEvent) => {
		e.preventDefault();
		if (!validate()) return;

		const res = await postReport(
			{
				itemId: String(itemId),
				reason: reason,
				itemType: itemType,
			},
			{
				RequestVerificationToken: props.csrf,
			},
		);
		success = res.ok;
	};

	const updateText = (e: InputEvent) => {
		reason = (e.currentTarget as HTMLTextAreaElement).value;
	};

	return (
		<Dialog ref={$set(dialogRef)} header={<span>Report</span>}>
			<form class="form" onSubmit={submit}>
				<div class="o-form-group">
					<label for="reason">Reason</label>

					<textarea
						name="reason"
						id="reason"
						class="o-form-control active-border"
						rows="5"
						minLength={rules.min}
						maxLength={rules.max}
						onInput={updateText}
					/>

					<div class="counter" classList={{ invalid: !validate() }}>
						<div
							class="o-progress-bar"
							style={{ width: `${Math.min(100, 100 * (chars() / rules.max))}%` }}
						/>
						<span>
							{chars()}/{rules.max} chars
						</span>
					</div>
				</div>

				<div class="o-form-group">
					<button type="submit" class={`btn ${message().cls}`}>
						{message().msg}
					</button>
				</div>
			</form>
		</Dialog>
	);
};

component(
	"report-modal",
	{
		openSelector: undefined,
		csrf: "",
		itemId: 0,
		itemType: "" as EReportableContentTypes,
	},
	ReportModal,
	[],
	["openSelector"],
);
