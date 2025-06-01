import { Report } from "@g/ctconfig";
import { PostApiReports as postReport } from "@g/paths-public";
import type { EReportableContentTypes } from "@g/types-public";
import { customElement, noShadowDOM } from "solid-element";
import { createSignal, onMount, type ParentComponent } from "solid-js";
import { Dialog, type DialogApi } from "./common/_dialog";

const ReportModal: ParentComponent<{
	openSelector: string;
	csrf: string;
	itemId: number;
	itemType: EReportableContentTypes;
}> = (props) => {
	noShadowDOM();

	const rules = {
		min: Report.MinReasonLength,
		max: Report.MaxReasonLength,
	};

	const [dialogRef, setDialogRef] = createSignal<DialogApi>();
	const [reason, setReason] = createSignal("");
	const [success, setSuccess] = createSignal<boolean | null>(null);

	onMount(() => {
		if (props.openSelector) {
			const element = document.querySelector(props.openSelector);
			if (element) {
				element.addEventListener("click", show);
			} else {
				console.log(`Element for selector ${props.openSelector} not found`);
			}
		}
	});

	const show = () => {
		dialogRef()?.open();
	};

	const validate = () => reason() && reason().length >= rules.min && reason().length <= rules.max;
	const chars = () => reason().length;

	const message = () => {
		switch (success()) {
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
				itemId: props.itemId,
				reason: reason(),
				itemType: props.itemType,
			},
			{
				RequestVerificationToken: props.csrf,
			},
		);
		setSuccess(res.ok);
	};

	const updateText = (e: InputEvent) => {
		const value = (e.currentTarget as HTMLTextAreaElement).value;
		setReason(value);
	};

	return (
		<Dialog ref={setDialogRef} header={<span>Report</span>}>
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

customElement(
	"report-modal",
	{
		openSelector: "",
		csrf: "",
		itemId: 0,
		itemType: "" as EReportableContentTypes,
	},
	ReportModal,
);
