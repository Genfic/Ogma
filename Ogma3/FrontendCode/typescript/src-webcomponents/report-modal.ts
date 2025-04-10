import { PostApiReports as postReport } from "@g/paths-public";
import type { EReportableContentTypes } from "@g/types-public";
import { LitElement, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { when } from "lit/directives/when.js";

@customElement("report-modal")
export class ReportModal extends LitElement {
	connectedCallback() {
		super.connectedCallback();
		if (this.openSelector) {
			const element = document.querySelector(this.openSelector);
			if (element) {
				element.addEventListener("click", this.show);
			} else {
				console.log(`Element is ${element} for selector ${this.openSelector}`);
			}
		}
	}

	private readonly rules = {
		min: 50,
		max: 500,
	};

	@property() public accessor openSelector: string | undefined;

	@property() public accessor csrf: string;
	@property() public accessor itemId: number;
	@property() public accessor itemType: string;

	@state() private accessor visible: boolean;
	@state() private accessor chars = 0;
	@state() private accessor reason: string;
	@state() private accessor success: boolean | null = null;

	public show = () => {
		this.visible = true;
	};

	private validate = () =>
		this.reason && this.reason.length >= this.rules.min && this.reason.length <= this.rules.max;

	private message = () => {
		switch (this.success) {
			case true:
				return { msg: "Report delivered!", cls: "green" };
			case false:
				return { msg: "An error has occurred. Retry?", cls: "red" };
			default:
				return { msg: "Send report", cls: null };
		}
	};

	// eslint-disable-next-line no-undef
	private submit = async (e: SubmitEvent) => {
		e.preventDefault();
		const res = await postReport(
			{
				itemId: this.itemId,
				reason: this.reason,
				itemType: this.itemType as EReportableContentTypes,
			},
			{
				RequestVerificationToken: this.csrf,
			},
		);
		this.success = res.ok;
	};

	private updateText = (e: InputEvent) => {
		this.reason = (e.currentTarget as HTMLTextAreaElement).value;
		this.chars = this.reason.length;
	};

	protected render() {
		return when(
			this.visible,
			() => html`
				<div
					class="club-folder-selector my-modal"
					@click="${() => {
						this.visible = false;
					}}"
				>
					<div class="content" @click="${(e: Event) => e.stopPropagation()}">
						<div class="header">
							<span>Report</span>
						</div>

						<form class="form" @submit="${this.submit}">
							<div class="o-form-group">
								<label for="reason">Reason</label>

								<textarea
									name="reason"
									id="reason"
									class="o-form-control active-border"
									rows="5"
									@input="${this.updateText}"
								></textarea>

								<div class="counter ${this.validate() || "invalid"}">
									<div
										class="o-progress-bar"
										style="width: ${`${Math.min(100, 100 * (this.chars / this.rules.max))}%`}"
									></div>
									<span>${this.chars}/${this.rules.max} chars</span>
								</div>

								${when(
									!this.validate(),
									() =>
										html`
											<span>
												Reason must be between ${this.rules.min} and ${this.rules.max} characters long.
											</span>`,
								)}
							</div>

							<div class="o-form-group">
								<button type="submit" class="btn ${this.message().cls}">${this.message().msg}</button>
							</div>
						</form>
					</div>
				</div>
			`,
		);
	}

	createRenderRoot() {
		return this;
	}
}
