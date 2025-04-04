﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Ogma3.Data.Bases;
using Ogma3.Data.Icons;
using Ogma3.Data.Shelves;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;

#pragma warning disable 219, 612, 618
#nullable disable

namespace CompiledModels
{
    [EntityFrameworkInternal]
    public partial class ShelfEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Ogma3.Data.Shelves.Shelf",
                typeof(Shelf),
                baseEntityType,
                propertyCount: 10,
                navigationCount: 2,
                skipNavigationCount: 1,
                foreignKeyCount: 2,
                unnamedIndexCount: 2,
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

            var color = runtimeEntityType.AddProperty(
                "Color",
                typeof(string),
                propertyInfo: typeof(Shelf).GetProperty("Color", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Shelf).GetField("<Color>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 7);
            color.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

            var description = runtimeEntityType.AddProperty(
                "Description",
                typeof(string),
                propertyInfo: typeof(Shelf).GetProperty("Description", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Shelf).GetField("<Description>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                maxLength: 100);
            description.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);
            description.AddAnnotation("Relational:DefaultValue", "");

            var iconId = runtimeEntityType.AddProperty(
                "IconId",
                typeof(long?),
                propertyInfo: typeof(Shelf).GetProperty("IconId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Shelf).GetField("<IconId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            iconId.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

            var isDefault = runtimeEntityType.AddProperty(
                "IsDefault",
                typeof(bool),
                propertyInfo: typeof(Shelf).GetProperty("IsDefault", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Shelf).GetField("<IsDefault>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                sentinel: false);
            isDefault.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);
            isDefault.AddAnnotation("Relational:DefaultValue", false);

            var isPublic = runtimeEntityType.AddProperty(
                "IsPublic",
                typeof(bool),
                propertyInfo: typeof(Shelf).GetProperty("IsPublic", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Shelf).GetField("<IsPublic>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                sentinel: false);
            isPublic.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);
            isPublic.AddAnnotation("Relational:DefaultValue", false);

            var isQuickAdd = runtimeEntityType.AddProperty(
                "IsQuickAdd",
                typeof(bool),
                propertyInfo: typeof(Shelf).GetProperty("IsQuickAdd", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Shelf).GetField("<IsQuickAdd>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                sentinel: false);
            isQuickAdd.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);
            isQuickAdd.AddAnnotation("Relational:DefaultValue", false);

            var name = runtimeEntityType.AddProperty(
                "Name",
                typeof(string),
                propertyInfo: typeof(Shelf).GetProperty("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Shelf).GetField("<Name>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20);
            name.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

            var ownerId = runtimeEntityType.AddProperty(
                "OwnerId",
                typeof(long),
                propertyInfo: typeof(Shelf).GetProperty("OwnerId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Shelf).GetField("<OwnerId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0L);
            ownerId.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

            var trackUpdates = runtimeEntityType.AddProperty(
                "TrackUpdates",
                typeof(bool),
                propertyInfo: typeof(Shelf).GetProperty("TrackUpdates", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Shelf).GetField("<TrackUpdates>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                sentinel: false);
            trackUpdates.AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);
            trackUpdates.AddAnnotation("Relational:DefaultValue", false);

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { iconId });

            var index0 = runtimeEntityType.AddIndex(
                new[] { ownerId });

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("IconId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.SetNull);

            var icon = declaringEntityType.AddNavigation("Icon",
                runtimeForeignKey,
                onDependent: true,
                typeof(Icon),
                propertyInfo: typeof(Shelf).GetProperty("Icon", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Shelf).GetField("<Icon>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("OwnerId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var owner = declaringEntityType.AddNavigation("Owner",
                runtimeForeignKey,
                onDependent: true,
                typeof(OgmaUser),
                propertyInfo: typeof(Shelf).GetProperty("Owner", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Shelf).GetField("<Owner>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            return runtimeForeignKey;
        }

        public static RuntimeSkipNavigation CreateSkipNavigation1(RuntimeEntityType declaringEntityType, RuntimeEntityType targetEntityType, RuntimeEntityType joinEntityType)
        {
            var skipNavigation = declaringEntityType.AddSkipNavigation(
                "Stories",
                targetEntityType,
                joinEntityType.FindForeignKey(
                    new[] { joinEntityType.FindProperty("ShelfId") },
                    declaringEntityType.FindKey(new[] { declaringEntityType.FindProperty("Id") }),
                    declaringEntityType),
                true,
                false,
                typeof(ICollection<Story>),
                propertyInfo: typeof(Shelf).GetProperty("Stories", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Shelf).GetField("<Stories>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var inverse = targetEntityType.FindSkipNavigation("Shelves");
            if (inverse != null)
            {
                skipNavigation.Inverse = inverse;
                inverse.Inverse = skipNavigation;
            }

            return skipNavigation;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "Shelves");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
