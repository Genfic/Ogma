using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class FixedForeignRatingKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Stories",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 6, 4, 7, 53, 398, DateTimeKind.Local).AddTicks(7861),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2019, 10, 6, 4, 5, 42, 26, DateTimeKind.Local).AddTicks(4773));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Stories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 6, 4, 5, 42, 26, DateTimeKind.Local).AddTicks(4773),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 6, 4, 7, 53, 398, DateTimeKind.Local).AddTicks(7861));
        }
    }
}
