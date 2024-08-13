export type None = undefined;

export interface GetTableInfoResponse {
    name: string;
    size: number;
}

export interface GetUserInfractionsResult {
    id: number;
    activeUntil: string;
    removed: boolean;
    reason: string;
}

export interface GetInfractionDetailsResult {
    id: number;
    userName: string;
    userId: number;
    issueDate: string;
    activeUntil: string;
    removedAt: string | null;
    reason: string;
    type: InfractionType;
    issuedByName: string;
    removedByName: string | null;
}

export interface InfractionType {}

export interface CreateInfractionResponse {
    id: number;
    issuedBy: number;
    issuedAgainst: number;
}

export interface CreateInfractionCommand {
    userId: number;
    reason: string;
    endDate: string;
    type: InfractionType;
}

export interface ProblemDetails {
    type: string | null;
    title: string | null;
    status: number | null;
    detail: string | null;
    instance: string | null;
}

export interface DeactivateInfractionResponse {
    id: number;
    issuedBy: number;
    issuedAgainst: number;
}