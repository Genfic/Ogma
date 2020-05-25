// Compile-time config variables

namespace Ogma3.Data
{
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
            public const int MinNameLength = 5;
            public const int MaxNameLength = 20;

            public const int MaxDescLength = 100;
        }
        
        public static class CNamespace
        {
            public const int MinNameLength = 5;
            public const int MaxNameLength = 10;
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
        }

        public static class CClub
        {
            public const int MinNameLength = 3;
            public const int MaxNameLength = 50;

            public const int MaxHookLength = 100;
            
            public const int MaxDescriptionLength = 10_000;
        }

        public static class CClubThread
        {
            public const int MinTitleLength = 3;
            public const int MaxTitleLength = 100;

            public const int MinBodyLength = 20;
            public const int MaxBodyLength = 25_000;
        }

        public static class CClubThreadComment
        {
            public const int MinBodyLength = 3;
            public const int MaxBodyLength = 5000;
        }
    }
}