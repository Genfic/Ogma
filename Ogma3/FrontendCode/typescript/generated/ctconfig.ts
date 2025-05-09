export const MaxEmailAddressLength = 254 as const;

export const Files = {
    AvatarMaxWeight: 1048576,
} as const;

export const User = {
    MinNameLength: 5,
    MaxNameLength: 20,
    MinPassLength: 10,
    MaxPassLength: 200,
    MaxTitleLength: 20,
    MaxBioLength: 10000,
    MaxLinksAmount: 5,
} as const;

export const Tag = {
    MinNameLength: 3,
    MaxNameLength: 20,
    MaxDescLength: 100,
} as const;

export const Rating = {
    MinNameLength: 4,
    MaxNameLength: 20,
    MinDescriptionLength: 5,
    MaxDescriptionLength: 1000,
} as const;

export const Chapter = {
    MinTitleLength: 5,
    MaxTitleLength: 100,
    MinBodyLength: 5000,
    MaxBodyLength: 500000,
    MaxNotesLength: 500,
    ValidateLengthMsg: 'The {0} must be at least {2} and at most {1} characters long.',
    ValidateNoteLengthMsg: 'The {0} cannot exceed {1} characters.',
} as const;

export const Story = {
    MinTitleLength: 3,
    MaxTitleLength: 100,
    MinDescriptionLength: 100,
    MaxDescriptionLength: 3000,
    MinHookLength: 50,
    MaxHookLength: 250,
    CoverMaxWeight: 2097152,
    MinTagCount: 3,
    ValidateLengthMsg: 'The {0} must be at least {2} and at most {1} characters long.',
    ValidateFileWeight: 'The {0} must be less than {1} bytes',
    ValidateTagCount: 'You must select {0} tags or more',
} as const;

export const Comment = {
    MinBodyLength: 2,
    MaxBodyLength: 5000,
} as const;

export const Shelf = {
    MinNameLength: 3,
    MaxNameLength: 20,
    MaxDescriptionLength: 100,
} as const;

export const Blogpost = {
    MinTitleLength: 3,
    MaxTitleLength: 100,
    MinBodyLength: 50,
    MaxBodyLength: 500000,
    ValidateLengthMsg: 'The {0} must be at least {2} and at most {1} characters long.',
    MaxTagsAmount: 10,
    MaxTagLength: 20,
    ValidateTagsCountMsg: 'The {0} can have a maximum of {1} tags.',
} as const;

export const Club = {
    MinNameLength: 3,
    MaxNameLength: 50,
    MinHookLength: 10,
    MaxHookLength: 100,
    MaxDescriptionLength: 25000,
    CoverMaxWeight: 2097152,
    ValidateLengthMsg: 'The {0} must be at least {2} and at most {1} characters long.',
    ValidateFileWeight: 'The {0} must be less than {1} bytes',
} as const;

export const ClubThread = {
    MinTitleLength: 3,
    MaxTitleLength: 100,
    MinBodyLength: 20,
    MaxBodyLength: 25000,
} as const;

export const Folder = {
    MinNameLength: 3,
    MaxNameLength: 20,
    MaxDescriptionLength: 500,
    ValidateLengthMsg: 'The {0} must be at least {2} and at most {1} characters long.',
} as const;

export const ClubThreadComment = {
    MinBodyLength: 3,
    MaxBodyLength: 5000,
} as const;

export const Report = {
    MinReasonLength: 30,
    MaxReasonLength: 500,
} as const;