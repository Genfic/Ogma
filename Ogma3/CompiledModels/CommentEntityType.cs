﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using Ogma3.Data.Bases;
using Ogma3.Data.Comments;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Users;

#pragma warning disable 219, 612, 618
#nullable disable

namespace CompiledModels
{
    internal partial class CommentEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Ogma3.Data.Comments.Comment",
                typeof(Comment),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(long),
                propertyInfo: typeof(BaseModel).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BaseModel).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: 0L);
            id.TypeMapping = LongTypeMapping.Default.Clone(
                comparer: new ValueComparer<long>(
                    (long v1, long v2) => v1 == v2,
                    (long v) => v.GetHashCode(),
                    (long v) => v),
                keyComparer: new ValueComparer<long>(
                    (long v1, long v2) => v1 == v2,
                    (long v) => v.GetHashCode(),
                    (long v) => v),
                providerValueComparer: new ValueComparer<long>(
                    (long v1, long v2) => v1 == v2,
                    (long v) => v.GetHashCode(),
                    (long v) => v));
            id.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            var authorId = runtimeEntityType.AddProperty(
                "AuthorId",
                typeof(long),
                propertyInfo: typeof(Comment).GetProperty("AuthorId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Comment).GetField("<AuthorId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0L);
            authorId.TypeMapping = LongTypeMapping.Default.Clone(
                comparer: new ValueComparer<long>(
                    (long v1, long v2) => v1 == v2,
                    (long v) => v.GetHashCode(),
                    (long v) => v),
                keyComparer: new ValueComparer<long>(
                    (long v1, long v2) => v1 == v2,
                    (long v) => v.GetHashCode(),
                    (long v) => v),
                providerValueComparer: new ValueComparer<long>(
                    (long v1, long v2) => v1 == v2,
                    (long v) => v.GetHashCode(),
                    (long v) => v));
            authorId.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

            var body = runtimeEntityType.AddProperty(
                "Body",
                typeof(string),
                propertyInfo: typeof(Comment).GetProperty("Body", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Comment).GetField("<Body>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 5000);
            body.TypeMapping = NpgsqlStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                keyComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "character varying(5000)",
                    size: 5000));
            body.TypeMapping = ((NpgsqlStringTypeMapping)body.TypeMapping).Clone(npgsqlDbType: NpgsqlTypes.NpgsqlDbType.Varchar);
        body.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

        var commentsThreadId = runtimeEntityType.AddProperty(
            "CommentsThreadId",
            typeof(long),
            propertyInfo: typeof(Comment).GetProperty("CommentsThreadId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            fieldInfo: typeof(Comment).GetField("<CommentsThreadId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            sentinel: 0L);
        commentsThreadId.TypeMapping = LongTypeMapping.Default.Clone(
            comparer: new ValueComparer<long>(
                (long v1, long v2) => v1 == v2,
                (long v) => v.GetHashCode(),
                (long v) => v),
            keyComparer: new ValueComparer<long>(
                (long v1, long v2) => v1 == v2,
                (long v) => v.GetHashCode(),
                (long v) => v),
            providerValueComparer: new ValueComparer<long>(
                (long v1, long v2) => v1 == v2,
                (long v) => v.GetHashCode(),
                (long v) => v));
        commentsThreadId.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

        var dateTime = runtimeEntityType.AddProperty(
            "DateTime",
            typeof(DateTime),
            propertyInfo: typeof(Comment).GetProperty("DateTime", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            fieldInfo: typeof(Comment).GetField("<DateTime>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            valueGenerated: ValueGenerated.OnAdd,
            sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        dateTime.TypeMapping = NpgsqlTimestampTypeMapping.Default.Clone(
            comparer: new ValueComparer<DateTime>(
                (DateTime v1, DateTime v2) => v1.Equals(v2),
                (DateTime v) => v.GetHashCode(),
                (DateTime v) => v),
            keyComparer: new ValueComparer<DateTime>(
                (DateTime v1, DateTime v2) => v1.Equals(v2),
                (DateTime v) => v.GetHashCode(),
                (DateTime v) => v),
            providerValueComparer: new ValueComparer<DateTime>(
                (DateTime v1, DateTime v2) => v1.Equals(v2),
                (DateTime v) => v.GetHashCode(),
                (DateTime v) => v));
        dateTime.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);
        dateTime.AddAnnotation("Relational:DefaultValueSql", "CURRENT_TIMESTAMP");

        var deletedBy = runtimeEntityType.AddProperty(
            "DeletedBy",
            typeof(EDeletedBy?),
            propertyInfo: typeof(Comment).GetProperty("DeletedBy", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            fieldInfo: typeof(Comment).GetField("<DeletedBy>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            nullable: true);
        deletedBy.TypeMapping = NpgsqlEnumTypeMapping.Default.Clone(
            comparer: new ValueComparer<EDeletedBy?>(
                (Nullable<EDeletedBy> v1, Nullable<EDeletedBy> v2) => v1.HasValue && v2.HasValue && object.Equals((object)(EDeletedBy)v1, (object)(EDeletedBy)v2) || !v1.HasValue && !v2.HasValue,
                (Nullable<EDeletedBy> v) => v.HasValue ? ((EDeletedBy)v).GetHashCode() : 0,
                (Nullable<EDeletedBy> v) => v.HasValue ? (Nullable<EDeletedBy>)(EDeletedBy)v : default(Nullable<EDeletedBy>)),
            keyComparer: new ValueComparer<EDeletedBy?>(
                (Nullable<EDeletedBy> v1, Nullable<EDeletedBy> v2) => v1.HasValue && v2.HasValue && object.Equals((object)(EDeletedBy)v1, (object)(EDeletedBy)v2) || !v1.HasValue && !v2.HasValue,
                (Nullable<EDeletedBy> v) => v.HasValue ? ((EDeletedBy)v).GetHashCode() : 0,
                (Nullable<EDeletedBy> v) => v.HasValue ? (Nullable<EDeletedBy>)(EDeletedBy)v : default(Nullable<EDeletedBy>)),
            providerValueComparer: new ValueComparer<EDeletedBy?>(
                (Nullable<EDeletedBy> v1, Nullable<EDeletedBy> v2) => v1.HasValue && v2.HasValue && object.Equals((object)(EDeletedBy)v1, (object)(EDeletedBy)v2) || !v1.HasValue && !v2.HasValue,
                (Nullable<EDeletedBy> v) => v.HasValue ? ((EDeletedBy)v).GetHashCode() : 0,
                (Nullable<EDeletedBy> v) => v.HasValue ? (Nullable<EDeletedBy>)(EDeletedBy)v : default(Nullable<EDeletedBy>)),
            mappingInfo: new RelationalTypeMappingInfo(
                storeTypeName: "e_deleted_by"),
            clrType: typeof(EDeletedBy),
            jsonValueReaderWriter: new NpgsqlEnumTypeMapping.JsonPgEnumReaderWriter<EDeletedBy>());
        deletedBy.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

        var deletedByUserId = runtimeEntityType.AddProperty(
            "DeletedByUserId",
            typeof(long?),
            propertyInfo: typeof(Comment).GetProperty("DeletedByUserId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            fieldInfo: typeof(Comment).GetField("<DeletedByUserId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            nullable: true);
        deletedByUserId.TypeMapping = LongTypeMapping.Default.Clone(
            comparer: new ValueComparer<long?>(
                (Nullable<long> v1, Nullable<long> v2) => v1.HasValue && v2.HasValue && (long)v1 == (long)v2 || !v1.HasValue && !v2.HasValue,
                (Nullable<long> v) => v.HasValue ? ((long)v).GetHashCode() : 0,
                (Nullable<long> v) => v.HasValue ? (Nullable<long>)(long)v : default(Nullable<long>)),
            keyComparer: new ValueComparer<long?>(
                (Nullable<long> v1, Nullable<long> v2) => v1.HasValue && v2.HasValue && (long)v1 == (long)v2 || !v1.HasValue && !v2.HasValue,
                (Nullable<long> v) => v.HasValue ? ((long)v).GetHashCode() : 0,
                (Nullable<long> v) => v.HasValue ? (Nullable<long>)(long)v : default(Nullable<long>)),
            providerValueComparer: new ValueComparer<long?>(
                (Nullable<long> v1, Nullable<long> v2) => v1.HasValue && v2.HasValue && (long)v1 == (long)v2 || !v1.HasValue && !v2.HasValue,
                (Nullable<long> v) => v.HasValue ? ((long)v).GetHashCode() : 0,
                (Nullable<long> v) => v.HasValue ? (Nullable<long>)(long)v : default(Nullable<long>)));
        deletedByUserId.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

        var editCount = runtimeEntityType.AddProperty(
            "EditCount",
            typeof(ushort),
            propertyInfo: typeof(Comment).GetProperty("EditCount", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            fieldInfo: typeof(Comment).GetField("<EditCount>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            valueGenerated: ValueGenerated.OnAdd);
        editCount.TypeMapping = IntTypeMapping.Default.Clone(
            comparer: new ValueComparer<ushort>(
                (ushort v1, ushort v2) => v1 == v2,
                (ushort v) => (int)v,
                (ushort v) => v),
            keyComparer: new ValueComparer<ushort>(
                (ushort v1, ushort v2) => v1 == v2,
                (ushort v) => (int)v,
                (ushort v) => v),
            providerValueComparer: new ValueComparer<int>(
                (int v1, int v2) => v1 == v2,
                (int v) => v,
                (int v) => v),
            mappingInfo: new RelationalTypeMappingInfo(
                storeTypeName: "integer"),
            converter: new ValueConverter<ushort, int>(
                (ushort v) => (int)v,
                (int v) => (ushort)v),
            jsonValueReaderWriter: new JsonConvertedValueReaderWriter<ushort, int>(
                JsonInt32ReaderWriter.Instance,
                new ValueConverter<ushort, int>(
                    (ushort v) => (int)v,
                    (int v) => (ushort)v)));
        editCount.SetSentinelFromProviderValue(0);
        editCount.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);
        editCount.AddAnnotation("Relational:DefaultValue", (ushort)0);

        var lastEdit = runtimeEntityType.AddProperty(
            "LastEdit",
            typeof(DateTime?),
            propertyInfo: typeof(Comment).GetProperty("LastEdit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            fieldInfo: typeof(Comment).GetField("<LastEdit>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            nullable: true);
        lastEdit.TypeMapping = NpgsqlTimestampTypeMapping.Default.Clone(
            comparer: new ValueComparer<DateTime?>(
                (Nullable<DateTime> v1, Nullable<DateTime> v2) => v1.HasValue && v2.HasValue && (DateTime)v1 == (DateTime)v2 || !v1.HasValue && !v2.HasValue,
                (Nullable<DateTime> v) => v.HasValue ? ((DateTime)v).GetHashCode() : 0,
                (Nullable<DateTime> v) => v.HasValue ? (Nullable<DateTime>)(DateTime)v : default(Nullable<DateTime>)),
            keyComparer: new ValueComparer<DateTime?>(
                (Nullable<DateTime> v1, Nullable<DateTime> v2) => v1.HasValue && v2.HasValue && (DateTime)v1 == (DateTime)v2 || !v1.HasValue && !v2.HasValue,
                (Nullable<DateTime> v) => v.HasValue ? ((DateTime)v).GetHashCode() : 0,
                (Nullable<DateTime> v) => v.HasValue ? (Nullable<DateTime>)(DateTime)v : default(Nullable<DateTime>)),
            providerValueComparer: new ValueComparer<DateTime?>(
                (Nullable<DateTime> v1, Nullable<DateTime> v2) => v1.HasValue && v2.HasValue && (DateTime)v1 == (DateTime)v2 || !v1.HasValue && !v2.HasValue,
                (Nullable<DateTime> v) => v.HasValue ? ((DateTime)v).GetHashCode() : 0,
                (Nullable<DateTime> v) => v.HasValue ? (Nullable<DateTime>)(DateTime)v : default(Nullable<DateTime>)));
        lastEdit.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

        var key = runtimeEntityType.AddKey(
            new[] { id });
        runtimeEntityType.SetPrimaryKey(key);

        var index = runtimeEntityType.AddIndex(
            new[] { authorId });

        var index0 = runtimeEntityType.AddIndex(
            new[] { commentsThreadId });

        var index1 = runtimeEntityType.AddIndex(
            new[] { deletedByUserId });

        return runtimeEntityType;
    }

    public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
    {
        var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("AuthorId") },
            principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
            principalEntityType,
            deleteBehavior: DeleteBehavior.Cascade,
            required: true);

        var author = declaringEntityType.AddNavigation("Author",
            runtimeForeignKey,
            onDependent: true,
            typeof(OgmaUser),
            propertyInfo: typeof(Comment).GetProperty("Author", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            fieldInfo: typeof(Comment).GetField("<Author>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

        return runtimeForeignKey;
    }

    public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
    {
        var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("CommentsThreadId") },
            principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
            principalEntityType,
            deleteBehavior: DeleteBehavior.Cascade,
            required: true);

        var commentsThread = declaringEntityType.AddNavigation("CommentsThread",
            runtimeForeignKey,
            onDependent: true,
            typeof(CommentsThread),
            propertyInfo: typeof(Comment).GetProperty("CommentsThread", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            fieldInfo: typeof(Comment).GetField("<CommentsThread>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

        var comments = principalEntityType.AddNavigation("Comments",
            runtimeForeignKey,
            onDependent: false,
            typeof(IList<Comment>),
            propertyInfo: typeof(CommentsThread).GetProperty("Comments", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            fieldInfo: typeof(CommentsThread).GetField("<Comments>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

        return runtimeForeignKey;
    }

    public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
    {
        var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("DeletedByUserId") },
            principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
            principalEntityType);

        var deletedByUser = declaringEntityType.AddNavigation("DeletedByUser",
            runtimeForeignKey,
            onDependent: true,
            typeof(OgmaUser),
            propertyInfo: typeof(Comment).GetProperty("DeletedByUser", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            fieldInfo: typeof(Comment).GetField("<DeletedByUser>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

        return runtimeForeignKey;
    }

    public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
    {
        runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
        runtimeEntityType.AddAnnotation("Relational:Schema", null);
        runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
        runtimeEntityType.AddAnnotation("Relational:TableName", "Comments");
        runtimeEntityType.AddAnnotation("Relational:ViewName", null);
        runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

        Customize(runtimeEntityType);
    }

    static partial void Customize(RuntimeEntityType runtimeEntityType);
}
}