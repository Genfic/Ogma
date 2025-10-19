export type CreateInfractionCommand = {
    userId: number;
    reason: string;
    endDate: Date;
    type: InfractionType;
};

export type GetUserDataInfractionDto = {
    id: number;
    issueDate: Date;
    activeUntil: Date;
    removedAt: Date | null;
    reason: string;
    type: InfractionType;
    removedBy: string | null;
};

export type GetUserDataUserDetailsDto = {
    id: number;
    name: string;
    email: string;
    title: string | null;
    avatar: string | null;
    registrationDate: Date;
    lastActive: Date;
    storiesCount: number;
    blogpostsCount: number;
    roleNames: string[];
    infractions: GetUserDataInfractionDto[];
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