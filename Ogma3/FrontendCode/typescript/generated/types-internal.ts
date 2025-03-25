export type CreateInfractionCommand = {
	userId: number;
	reason: string;
	endDate: string;
	type: InfractionType;
};

export type GetUserInfractionsResult = {
	id: number;
	activeUntil: string;
	removed: boolean;
	reason: string;
};

export type InfractionDto = {
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
};

export type InfractionType = "Note" | "Warning" | "Mute" | "Ban";

export type None = undefined;