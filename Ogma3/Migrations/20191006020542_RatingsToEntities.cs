using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class RatingsToEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Stories");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Stories",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 6, 4, 5, 42, 26, DateTimeKind.Local).AddTicks(4773),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2019, 10, 6, 3, 34, 20, 751, DateTimeKind.Local).AddTicks(996));

            migrationBuilder.AddColumn<int>(
                name: "RatingId",
                table: "Stories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Icon = table.Column<string>(nullable: true),
                    IconId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.UniqueConstraint("AK_Ratings_Name", x => x.Name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stories_RatingId",
                table: "Stories",
                column: "RatingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_Ratings_RatingId",
                table: "Stories",
                column: "RatingId",
                principalTable: "Ratings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stories_Ratings_RatingId",
                table: "Stories");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Stories_RatingId",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "RatingId",
                table: "Stories");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Stories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 6, 3, 34, 20, 751, DateTimeKind.Local).AddTicks(996),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 6, 4, 5, 42, 26, DateTimeKind.Local).AddTicks(4773));

            migrationBuilder.AddColumn<string>(
                name: "Rating",
                table: "Stories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Everyone");
        }
    }
}
