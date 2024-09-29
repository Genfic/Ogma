export interface CreateInfractionCommand {
	userId: number;
	reason: string;
	endDate: string;
	type: InfractionType;
}

export interface GetUserInfractionsResult {
	id: number;
	activeUntil: string;
	removed: boolean;
	reason: string;
}

export interface InfractionDto {
	id: number;
	userUserName: string;
	userId: number;
	issueDate: string;
	activeUntil: string;
	removedAt: string | null;
	reason: string;
	type: InfractionType;
	issuedByUserName: string;
	removedByUserName: string | null;
}

export type InfractionType = "Invalid" | "Note" | "Warning" | "Mute" | "Ban";

export type None = undefined;