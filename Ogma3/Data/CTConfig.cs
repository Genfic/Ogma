// Compile-time config variables

namespace Ogma3.Data
{
    // ReSharper disable once InconsistentNaming
    public static class CTConfig
    {
        public static class CFiles
        {
            public const int AvatarMaxWeight = 1 * 1024 * 1024;
        }
        
        public static class CUser
        {
            public const int MinNameLength = 5;
            public const int MaxNameLength = 20;

            public const int MinPassLength = 10;
            public const int MaxPassLength = 200;

            public const int MaxTitleLength = 20;
            public const int MaxBioLength = 10_000;
        }
        
        public static class CTag
        {
            public const int MinNameLength = 3;
            public const int MaxNameLength = 20;

            public const int MaxDescLength = 100;
        }
        
        public static class CRating
        {
            public const int MinNameLength = 5;
            public const int MaxNameLength = 20;

            public const int MinDescriptionLength = 5;
            public const int MaxDescriptionLength = 1000;
        }
        
        public static class CChapter
        {
            public const int MinTitleLength = 5;
            public const int MaxTitleLength = 100;

            public const int MinBodyLength = 5_000;
            public const int MaxBodyLength = 500_000;

            public const int MaxNotesLength = 500;
            
            public const string ValidateLengthMsg = "The {0} must be at least {2} and at most {1} characters long.";
            public const string ValidateNoteLengthMsg = "The {0} cannot exceed {1} characters.";
        }
        
        public static class CStory
        {
            public const int MinTitleLength = 3;
            public const int MaxTitleLength = 100;

            public const int MinDescriptionLength = 100;
            public const int MaxDescriptionLength = 3000;

            public const int MinHookLength = 50;
            public const int MaxHookLength = 250;
            public const int CoverMaxWeight = 2 * 1024 * 1024;

            public const int MinTagCount = 3;
            
            public const string ValidateLengthMsg = "The {0} must be at least {2} and at most {1} characters long.";
            public const string ValidateFileWeight = "The {0} must be less than {1} bytes";
            public const string ValidateTagCount = "You must select {0} tags or more";
        }
        
        public static class CComment
        {
            public const int MinBodyLength = 2;
            public const int MaxBodyLength = 5000;
        }

        public static class CShelf
        {
            public const int MinNameLength = 3;
            public const int MaxNameLength = 20;
            
            public const int MaxDescriptionLength = 100;
        }

        public static class CBlogpost
        {
            public const int MinTitleLength = 3;
            public const int MaxTitleLength = 100;

            public const int MinBodyLength = 50;
            public const int MaxBodyLength = 500_000;

            public const string ValidateLengthMsg = "The {0} must be at least {2} and at most {1} characters long.";
            
            public const int MaxTagsAmount = 10;
            public const int MaxTagLength = 20;
            public const string ValidateTagsCountMsg = "The {0} can have a maximum of {1} tags.";
        }

        public static class CClub
        {
            public const int MinNameLength = 3;
            public const int MaxNameLength = 50;

            public const int MinHookLength = 10;
            public const int MaxHookLength = 100;
            
            public const int MaxDescriptionLength = 25_000;
            
            public const int CoverMaxWeight = 2 * 1024 * 1024;
            
            public const string ValidateLengthMsg = "The {0} must be at least {2} and at most {1} characters long.";
            public const string ValidateFileWeight = "The {0} must be less than {1} bytes";
        }

        public static class CClubThread
        {
            public const int MinTitleLength = 3;
            public const int MaxTitleLength = 100;

            public const int MinBodyLength = 20;
            public const int MaxBodyLength = 25_000;
        }

        public static class CFolder
        {
            public const int MinNameLength = 3;
            public const int MaxNameLength = 20;
            
            public const int MaxDescriptionLength = 100;
            
            public const string ValidateLengthMsg = "The {0} must be at least {2} and at most {1} characters long.";
        }

        public static class CClubThreadComment
        {
            public const int MinBodyLength = 3;
            public const int MaxBodyLength = 5000;
        }
    }
}