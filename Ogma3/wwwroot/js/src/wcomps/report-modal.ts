import { html, LitElement } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { Reports_PostReports as postReport } from "../../generated/paths-public";
import { EReportableContentTypes } from "../../generated/types-public";

@customElement("report-modal")
export class ReportModal extends LitElement {
	constructor() {
		super();
	}

	connectedCallback() {
		super.connectedCallback();
		if (this.openSelector) {
			document.querySelector(this.openSelector).addEventListener('click', this.show);
		}
	}

	private readonly rules = {
		min: 50,
		max: 500,
	};
	
	@property() public openSelector?: string | undefined;
	
	@property() public csrf: string;
	@property() public itemId: number;
	@property() public itemType: string;

	@state() private visible: boolean;
	@state() private chars: number = 0;
	@state() private reason: string;
	@state() private success: boolean | null = null;
	
	public show = () => this.visible = true;

	private validate = () => this.reason && this.reason.length >= this.rules.min && this.reason.length <= this.rules.max;

	private message = () => {
		switch (this.success) {
			case true:
				return { msg: "Report delivered!", cls: 'green' };
			case false:
				return { msg: "An error has occurred. Retry?", cls: 'red' };
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
			}
		);
		this.success = res.ok;
	};

	private updateText = (e: InputEvent) => {
		this.reason = (e.currentTarget as HTMLTextAreaElement).value;
		this.chars = this.reason.length;
	};

	protected render() {
		return html`
			${this.visible
				? html`
						<div
							class="club-folder-selector my-modal"
							@click="${() => (this.visible = false)}"
						>
							<div
								class="content"
								@click="${(e) => e.stopPropagation()}"
							>
								<div class="header">
									<span>Report</span>
								</div>

								<form
									class="form"
									@submit="${this.submit}"
								>
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
												style="width: ${Math.min(100, 100 * (this.chars / this.rules.max)) + "%"}"
											></div>
											<span>${this.chars}/${this.rules.max} chars</span>
										</div>

										${this.validate()
											? null
											: html`<span>
													Reason must be between ${this.rules.min} and ${this.rules.max} characters long.
											  </span>`}
									</div>

									<div class="o-form-group">
										<button
											type="submit"
											class="btn ${this.message().cls}"
										>${this.message().msg}</button>
									</div>
								</form>
							</div>
						</div>
				  `
				: null}
		`;
	}

	createRenderRoot() {
		return this;
	}
}