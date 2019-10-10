using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class UpdatedDefaultDateGEn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Stories",
                nullable: false,
                defaultValueSql: "getdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2019, 10, 10, 5, 27, 6, 508, DateTimeKind.Local).AddTicks(3317));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishDate",
                table: "Chapters",
                nullable: false,
                defaultValueSql: "getdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2019, 10, 10, 5, 27, 6, 514, DateTimeKind.Local).AddTicks(1477));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Stories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 10, 5, 27, 6, 508, DateTimeKind.Local).AddTicks(3317),
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getdate()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishDate",
                table: "Chapters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 10, 5, 27, 6, 514, DateTimeKind.Local).AddTicks(1477),
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getdate()");
        }
    }
}
