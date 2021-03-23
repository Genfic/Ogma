using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ogma3.Migrations
{
    public partial class RatingOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Namespaces");

            migrationBuilder.AddColumn<byte>(
                name: "Order",
                table: "Ratings",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AlterColumn<string[]>(
                name: "Hashtags",
                table: "Blogposts",
                type: "text[]",
                maxLength: 10,
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldMaxLength: 10,
                oldDefaultValue: new string[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Ratings");

            migrationBuilder.AlterColumn<string[]>(
                name: "Hashtags",
                table: "Blogposts",
                type: "text[]",
                maxLength: 10,
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldMaxLength: 10,
                oldDefaultValue: new string[0]);

            migrationBuilder.CreateTable(
                name: "Namespaces",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    Name = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Namespaces", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Namespaces_Name",
                table: "Namespaces",
                column: "Name",
                unique: true);
        }
    }
}
