using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    /// <inheritdoc />
    public partial class NullableStoryCovers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stories_Images_CoverId",
                table: "Stories");

            migrationBuilder.AlterColumn<long>(
                name: "CoverId",
                table: "Stories",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_Images_CoverId",
                table: "Stories",
                column: "CoverId",
                principalTable: "Images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stories_Images_CoverId",
                table: "Stories");

            migrationBuilder.AlterColumn<long>(
                name: "CoverId",
                table: "Stories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_Images_CoverId",
                table: "Stories",
                column: "CoverId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
