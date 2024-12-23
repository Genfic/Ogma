﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Ogma3.Data.Bases;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Chapters;
using Ogma3.Data.Clubs;
using Ogma3.Data.Comments;
using Ogma3.Data.Reports;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;

#pragma warning disable 219, 612, 618
#nullable disable

namespace CompiledModels
{
    [EntityFrameworkInternal]
    public partial class ReportEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Ogma3.Data.Reports.Report",
                typeof(Report),
                baseEntityType,
                propertyCount: 11,
                navigationCount: 7,
                foreignKeyCount: 7,
                unnamedIndexCount: 7,
                keyCount: 1);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(long),
                propertyInfo: typeof(BaseModel).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BaseModel).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: 0L);
            id.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            var blogpostId = runtimeEntityType.AddProperty(
                "BlogpostId",
                typeof(long?),
                propertyInfo: typeof(Report).GetProperty("BlogpostId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<BlogpostId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            blogpostId.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

            var chapterId = runtimeEntityType.AddProperty(
                "ChapterId",
                typeof(long?),
                propertyInfo: typeof(Report).GetProperty("ChapterId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<ChapterId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            chapterId.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

            var clubId = runtimeEntityType.AddProperty(
                "ClubId",
                typeof(long?),
                propertyInfo: typeof(Report).GetProperty("ClubId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<ClubId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            clubId.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

            var commentId = runtimeEntityType.AddProperty(
                "CommentId",
                typeof(long?),
                propertyInfo: typeof(Report).GetProperty("CommentId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<CommentId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            commentId.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

            var contentType = runtimeEntityType.AddProperty(
                "ContentType",
                typeof(string),
                propertyInfo: typeof(Report).GetProperty("ContentType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<ContentType>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            contentType.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

            var reason = runtimeEntityType.AddProperty(
                "Reason",
                typeof(string),
                propertyInfo: typeof(Report).GetProperty("Reason", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<Reason>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            reason.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

            var reportDate = runtimeEntityType.AddProperty(
                "ReportDate",
                typeof(DateTimeOffset),
                propertyInfo: typeof(Report).GetProperty("ReportDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<ReportDate>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                sentinel: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
            reportDate.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);
            reportDate.AddAnnotation("Relational:DefaultValueSql", "CURRENT_TIMESTAMP");

            var reporterId = runtimeEntityType.AddProperty(
                "ReporterId",
                typeof(long),
                propertyInfo: typeof(Report).GetProperty("ReporterId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<ReporterId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0L);
            reporterId.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

            var storyId = runtimeEntityType.AddProperty(
                "StoryId",
                typeof(long?),
                propertyInfo: typeof(Report).GetProperty("StoryId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<StoryId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            storyId.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

            var userId = runtimeEntityType.AddProperty(
                "UserId",
                typeof(long?),
                propertyInfo: typeof(Report).GetProperty("UserId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<UserId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            userId.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { blogpostId });

            var index0 = runtimeEntityType.AddIndex(
                new[] { chapterId });

            var index1 = runtimeEntityType.AddIndex(
                new[] { clubId });

            var index2 = runtimeEntityType.AddIndex(
                new[] { commentId });

            var index3 = runtimeEntityType.AddIndex(
                new[] { reporterId });

            var index4 = runtimeEntityType.AddIndex(
                new[] { storyId });

            var index5 = runtimeEntityType.AddIndex(
                new[] { userId });

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("BlogpostId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade);

            var blogpost = declaringEntityType.AddNavigation("Blogpost",
                runtimeForeignKey,
                onDependent: true,
                typeof(Blogpost),
                propertyInfo: typeof(Report).GetProperty("Blogpost", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<Blogpost>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var reports = principalEntityType.AddNavigation("Reports",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Report>),
                propertyInfo: typeof(Blogpost).GetProperty("Reports", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Blogpost).GetField("<Reports>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ChapterId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade);

            var chapter = declaringEntityType.AddNavigation("Chapter",
                runtimeForeignKey,
                onDependent: true,
                typeof(Chapter),
                propertyInfo: typeof(Report).GetProperty("Chapter", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<Chapter>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var reports = principalEntityType.AddNavigation("Reports",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Report>),
                propertyInfo: typeof(Chapter).GetProperty("Reports", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Chapter).GetField("<Reports>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ClubId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade);

            var club = declaringEntityType.AddNavigation("Club",
                runtimeForeignKey,
                onDependent: true,
                typeof(Club),
                propertyInfo: typeof(Report).GetProperty("Club", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<Club>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var reports = principalEntityType.AddNavigation("Reports",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Report>),
                propertyInfo: typeof(Club).GetProperty("Reports", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Club).GetField("<Reports>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey4(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("CommentId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade);

            var comment = declaringEntityType.AddNavigation("Comment",
                runtimeForeignKey,
                onDependent: true,
                typeof(Comment),
                propertyInfo: typeof(Report).GetProperty("Comment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<Comment>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var reports = principalEntityType.AddNavigation("Reports",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Report>),
                propertyInfo: typeof(Comment).GetProperty("Reports", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Comment).GetField("<Reports>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey5(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ReporterId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var reporter = declaringEntityType.AddNavigation("Reporter",
                runtimeForeignKey,
                onDependent: true,
                typeof(OgmaUser),
                propertyInfo: typeof(Report).GetProperty("Reporter", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<Reporter>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey6(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("StoryId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade);

            var story = declaringEntityType.AddNavigation("Story",
                runtimeForeignKey,
                onDependent: true,
                typeof(Story),
                propertyInfo: typeof(Report).GetProperty("Story", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<Story>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var reports = principalEntityType.AddNavigation("Reports",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Report>),
                propertyInfo: typeof(Story).GetProperty("Reports", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Story).GetField("<Reports>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey7(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("UserId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade);

            var user = declaringEntityType.AddNavigation("User",
                runtimeForeignKey,
                onDependent: true,
                typeof(OgmaUser),
                propertyInfo: typeof(Report).GetProperty("User", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Report).GetField("<User>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var reports = principalEntityType.AddNavigation("Reports",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Report>),
                propertyInfo: typeof(OgmaUser).GetProperty("Reports", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OgmaUser).GetField("<Reports>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "Reports");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
