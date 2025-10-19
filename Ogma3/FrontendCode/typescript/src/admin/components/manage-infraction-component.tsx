import { PostAdminApiInfractions } from "@g/paths-internal";
import type { InfractionType } from "@g/types-internal";
import { type Component, createSignal, For, onMount } from "solid-js";
import { Dialog, type DialogApi } from "../../comp/common/_dialog";

export interface ManageInfractionApi {
	open: () => void;
}

interface ManageInfractionProps {
	csrf: string;
	userId: number;
	types: InfractionType[];
	ref?: (api: ManageInfractionApi) => void;
	onSuccess: () => void;
}

export const ManageInfraction: Component<ManageInfractionProps> = (props) => {
	const [type, setType] = createSignal<InfractionType | null>(null);
	const [date, setDate] = createSignal<string | null>(null);
	const [reason, setReason] = createSignal<string | null>(null);

	const dialogRef = $signal<DialogApi>();

	const create = async (e: SubmitEvent) => {
		e.preventDefault();

		const r = reason();
		const d = date();
		const t = type();

		if (!r) throw new Error("Reason is required");
		if (!d) throw new Error("Expiration date is required");
		if (!t) throw new Error("Infraction type is required");

		const res = await PostAdminApiInfractions(
			{
				userId: props.userId,
				reason: r,
				endDate: new Date(d),
				type: t,
			},
			{
				RequestVerificationToken: props.csrf,
			},
		);

		dialogRef?.close();
		setType(null);
		setDate(null);
		setReason(null);

		if (!res.ok) {
			throw res.error;
		}

		props.onSuccess();
	};

	onMount(() => {
		props.ref?.({
			open: () => {
				dialogRef?.open();
			},
		});
	});

	return (
		<Dialog ref={$set(dialogRef)} header={<strong>Create infraction</strong>}>
			<form class="form" onSubmit={create}>
				<div class="o-form-group">
					<label for="type">Infraction type</label>
					<select
						id="type"
						value={type() ?? ""}
						onInput={(e) => setType(e.currentTarget.value as InfractionType)}
						required
					>
						<option value="" disabled selected>
							Select a type...
						</option>
						<For each={props.types}>{(t) => <option value={t}>{t}</option>}</For>
					</select>
				</div>

				<div class="o-form-group">
					<label for="time">Expiration date</label>
					<input type="date" id="time" value={date() ?? ""} onInput={(e) => setDate(e.currentTarget.value)} />
				</div>

				<div class="o-form-group">
					<label for="reason">Reason</label>
					<textarea
						id="reason"
						value={reason() ?? ""}
						onInput={(e) => setReason(e.currentTarget.value)}
						required
					/>
				</div>

				<div class="o-form-group">
					<input type="submit" value="Create" />
				</div>
			</form>
		</Dialog>
	);
};
