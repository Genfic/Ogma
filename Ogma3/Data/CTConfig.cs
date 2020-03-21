// Compile-time config variables

namespace Ogma3.Data
{
    public static class CTConfig
    {
        public static class Files
        {
            public const int AvatarMaxWeight = 1 * 1024 * 1024;
        }
        
        public static class User
        {
            public const int MinNameLength = 5;
            public const int MaxNameLength = 20;

            public const int MinPassLength = 10;
            public const int MaxPassLength = 200;

            public const int MaxTitleLength = 20;
            public const int MaxBioLength = 10_000;
        }
        
        public static class Category
        {
            public const int MinNameLength = 5;
            public const int MaxNameLength = 20;

            public const int MinDescLength = 10;
            public const int MaxDescLength = 100;
        }
        
        public static class Tag
        {
            public const int MinNameLength = 5;
            public const int MaxNameLength = 20;

            public const int MaxDescLength = 100;
        }
        
        public static class Namespace
        {
            public const int MinNameLength = 5;
            public const int MaxNameLength = 10;
        }
        
        public static class Rating
        {
            public const int MinNameLength = 5;
            public const int MaxNameLength = 20;

            public const int MinDescriptionLength = 5;
            public const int MaxDescriptionLength = 1000;
        }
        
        public static class Chapter
        {
            public const int MinTitleLength = 5;
            public const int MaxTitleLength = 50;

            public const int MinBodyLength = 1000;
            public const int MaxBodyLength = 500_000;

            public const int MaxNotesLength = 500;
        }
        
        public static class Story
        {
            public const int MinTitleLength = 3;
            public const int MaxTitleLength = 50;

            public const int MinDescriptionLength = 100;
            public const int MaxDescriptionLength = 1500;

            public const int MinHookLength = 50;
            public const int MaxHookLength = 500;
            public const int CoverMaxWeight = 2 * 1024 * 1024;
        }
        
        public static class Comment
        {
            public const int MinBodyLength = 2;
            public const int MaxBodyLength = 10_000;
        }

        public static class Shelf
        {
            public const int MinNameLength = 3;
            public const int MaxNameLength = 20;
            
            public const int MaxDescriptionLength = 100;
        }
    }
}