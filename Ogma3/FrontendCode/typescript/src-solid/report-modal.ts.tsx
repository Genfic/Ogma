import { PostApiReports as postReport } from "@g/paths-public";
import type { EReportableContentTypes } from "@g/types-public";
import { customElement } from "solid-element";
import { Show, createSignal, onMount } from "solid-js";

customElement(
	"report-modal",
	{
		openSelector: String,
		csrf: String,
		itemId: Number,
		itemType: String,
	},
	(props) => {
		const rules = {
			min: 50,
			max: 500,
		};

		const [visible, setVisible] = createSignal(false);
		const [chars, setChars] = createSignal(0);
		const [reason, setReason] = createSignal("");
		const [success, setSuccess] = createSignal<boolean | null>(null);

		onMount(() => {
			if (props.openSelector) {
				const element = document.querySelector(props.openSelector);
				if (element) {
					element.addEventListener("click", show);
				} else {
					console.log(`Element is ${element} for selector ${props.openSelector}`);
				}
			}
		});

		const show = () => {
			setVisible(true);
		};

		const validate = () => reason() && reason().length >= rules.min && reason().length <= rules.max;

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
			const res = await postReport(
				{
					itemId: props.itemId,
					reason: reason(),
					itemType: props.itemType as EReportableContentTypes,
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
			setChars(value.length);
		};

		// Expose show method to element instance
		Object.assign(props.element, { show });

		return () => (
			<Show when={visible()}>
				<div class="club-folder-selector my-modal" onClick={() => setVisible(false)}>
					<div class="content" onClick={(e) => e.stopPropagation()}>
						<div class="header">
							<span>Report</span>
						</div>

						<form class="form" onSubmit={submit}>
							<div class="o-form-group">
								<label for="reason">Reason</label>

								<textarea
									name="reason"
									id="reason"
									class="o-form-control active-border"
									rows="5"
									onInput={updateText}
								/>

								<div class={`counter ${validate() ? "" : "invalid"}`}>
									<div
										class="o-progress-bar"
										style={{ width: `${Math.min(100, 100 * (chars() / rules.max))}%` }}
									/>
									<span>
										{chars()}/{rules.max} chars
									</span>
								</div>

								<Show when={!validate()}>
									<span>
										Reason must be between {rules.min} and {rules.max} characters long.
									</span>
								</Show>
							</div>

							<div class="o-form-group">
								<button type="button" type="submit" class={`btn ${message().cls}`}>
									{message().msg}
								</button>
							</div>
						</form>
					</div>
				</div>
			</Show>
		);
	},
);
