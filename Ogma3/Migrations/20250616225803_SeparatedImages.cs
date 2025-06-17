using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ogma3.Migrations
{
    /// <inheritdoc />
    public partial class SeparatedImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cover",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "AspNetUsers");

            // migrationBuilder.AlterColumn<long>(
            //     name: "CoverId",
            //     table: "Stories",
            //     type: "bigint",
            //     nullable: false,
            //     defaultValue: 0L,
            //     oldClrType: typeof(string),
            //     oldType: "text",
            //     oldNullable: true);
            migrationBuilder.DropColumn(
	            name: "CoverId",
	            table: "Stories"
            );
            migrationBuilder.AddColumn<long>(
	            name: "CoverId",
	            table: "Stories",
	            nullable: false
	        );

            // migrationBuilder.AlterColumn<long>(
            //     name: "IconId",
            //     table: "Clubs",
            //     type: "bigint",
            //     nullable: false,
            //     defaultValue: 0L,
            //     oldClrType: typeof(string),
            //     oldType: "text",
            //     oldNullable: true);
            migrationBuilder.DropColumn(
	            name: "IconId",
	            table: "Clubs"
            );
            migrationBuilder.AddColumn<long>(
	            name: "IconId",
	            table: "Clubs",
	            nullable: false
            );

            // migrationBuilder.AlterColumn<long>(
            //     name: "AvatarId",
            //     table: "AspNetUsers",
            //     type: "bigint",
            //     nullable: false,
            //     defaultValue: 0L,
            //     oldClrType: typeof(string),
            //     oldType: "text",
            //     oldNullable: true);
            migrationBuilder.DropColumn(
	            name: "AvatarId",
	            table: "AspNetUsers"
            );
            migrationBuilder.AddColumn<long>(
	            name: "AvatarId",
	            table: "AspNetUsers",
	            nullable: false
            );

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    BackblazeId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stories_CoverId",
                table: "Stories",
                column: "CoverId");

            migrationBuilder.CreateIndex(
                name: "IX_Clubs_IconId",
                table: "Clubs",
                column: "IconId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AvatarId",
                table: "AspNetUsers",
                column: "AvatarId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Images_AvatarId",
                table: "AspNetUsers",
                column: "AvatarId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clubs_Images_IconId",
                table: "Clubs",
                column: "IconId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_Images_CoverId",
                table: "Stories",
                column: "CoverId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Images_AvatarId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Clubs_Images_IconId",
                table: "Clubs");

            migrationBuilder.DropForeignKey(
                name: "FK_Stories_Images_CoverId",
                table: "Stories");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Stories_CoverId",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Clubs_IconId",
                table: "Clubs");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AvatarId",
                table: "AspNetUsers");

            // migrationBuilder.AlterColumn<string>(
            //     name: "CoverId",
            //     table: "Stories",
            //     type: "text",
            //     nullable: true,
            //     oldClrType: typeof(long),
            //     oldType: "bigint");
            migrationBuilder.DropColumn(
	            name: "CoverId",
	            table: "Stories"
            );
            migrationBuilder.AddColumn<string>(
	            name: "CoverId",
	            table: "Stories",
	            type: "text",
	            nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "Cover",
                table: "Stories",
                type: "text",
                nullable: false,
                defaultValue: "");

            // migrationBuilder.AlterColumn<string>(
            //     name: "IconId",
            //     table: "Clubs",
            //     type: "text",
            //     nullable: true,
            //     oldClrType: typeof(long),
            //     oldType: "bigint");
            migrationBuilder.DropColumn(
	            name: "IconId",
	            table: "Clubs"
            );
            migrationBuilder.AddColumn<string>(
	            name: "IconId",
	            table: "Clubs",
	            type: "text",
	            nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Clubs",
                type: "text",
                nullable: false,
                defaultValue: "");

            // migrationBuilder.AlterColumn<string>(
            //     name: "AvatarId",
            //     table: "AspNetUsers",
            //     type: "text",
            //     nullable: true,
            //     oldClrType: typeof(long),
            //     oldType: "bigint");
            migrationBuilder.DropColumn(
	            name: "AvatarId",
	            table: "AspNetUsers"
            );
            migrationBuilder.AddColumn<string>(
	            name: "AvatarId",
	            table: "AspNetUsers",
	            type: "text",
	            nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
