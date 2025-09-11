export type CreateInfractionCommand = {
    userId: number;
    reason: string;
    endDate: Date;
    type: InfractionType;
};

export type GetUserInfractionsResult = {
    id: number;
    activeUntil: Date;
    removed: boolean;
    reason: string;
};

export type InfractionDto = {
    id: number;
    userUserName: string;
    userId: number;
    issueDate: Date;
    activeUntil: Date;
    removedAt: Date | null;
    reason: string;
    type: InfractionType;
    issuedByUserName: string;
    removedByUserName: string | null;
};

export type InfractionType = "Ban" | "Mute" | "Note" | "Warning";

export type None = undefined;