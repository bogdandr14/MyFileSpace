using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFileSpace.Infrastructure.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddAlternateKeyTagName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_TagName",
                table: "User");

            migrationBuilder.AlterColumn<string>(
                name: "TagName",
                table: "User",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_User_TagName",
                table: "User",
                column: "TagName");

            migrationBuilder.CreateIndex(
                name: "IX_User_TagName",
                table: "User",
                column: "TagName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_User_TagName",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_TagName",
                table: "User");

            migrationBuilder.AlterColumn<string>(
                name: "TagName",
                table: "User",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_User_TagName",
                table: "User",
                column: "TagName",
                unique: true,
                filter: "[TagName] IS NOT NULL");
        }
    }
}
