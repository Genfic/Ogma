using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class AddedUniqueNameRequirementToNamespaces : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Namespace_NamespaceId",
                table: "Tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tag",
                table: "Tag");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Tag_Name",
                table: "Tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Namespace",
                table: "Namespace");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Namespace_Name",
                table: "Namespace");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Category_Name",
                table: "Category");

            migrationBuilder.RenameTable(
                name: "Tag",
                newName: "Tags");

            migrationBuilder.RenameTable(
                name: "Namespace",
                newName: "Namespaces");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_Tag_NamespaceId",
                table: "Tags",
                newName: "IX_Tags_NamespaceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Tags_Name",
                table: "Tags",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Namespaces",
                table: "Namespaces",
                column: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Namespaces_Name",
                table: "Namespaces",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Categories_Name",
                table: "Categories",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Namespaces_NamespaceId",
                table: "Tags",
                column: "NamespaceId",
                principalTable: "Namespaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Namespaces_NamespaceId",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Tags_Name",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Namespaces",
                table: "Namespaces");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Namespaces_Name",
                table: "Namespaces");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Categories_Name",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "Tag");

            migrationBuilder.RenameTable(
                name: "Namespaces",
                newName: "Namespace");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_NamespaceId",
                table: "Tag",
                newName: "IX_Tag_NamespaceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tag",
                table: "Tag",
                column: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Tag_Name",
                table: "Tag",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Namespace",
                table: "Namespace",
                column: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Namespace_Name",
                table: "Namespace",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Category_Name",
                table: "Category",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Namespace_NamespaceId",
                table: "Tag",
                column: "NamespaceId",
                principalTable: "Namespace",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
