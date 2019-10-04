using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class TagNamespaces : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NamespaceId",
                table: "Tag",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Namespace",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Namespace", x => x.Id);
                    table.UniqueConstraint("AK_Namespace_Name", x => x.Name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tag_NamespaceId",
                table: "Tag",
                column: "NamespaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Namespace_NamespaceId",
                table: "Tag",
                column: "NamespaceId",
                principalTable: "Namespace",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Namespace_NamespaceId",
                table: "Tag");

            migrationBuilder.DropTable(
                name: "Namespace");

            migrationBuilder.DropIndex(
                name: "IX_Tag_NamespaceId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "NamespaceId",
                table: "Tag");
        }
    }
}
