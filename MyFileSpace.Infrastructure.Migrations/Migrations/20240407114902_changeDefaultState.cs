using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFileSpace.Infrastructure.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class changeDefaultState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "VirtualDirectory");

            migrationBuilder.DropColumn(
                name: "State",
                table: "StoredFile");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "VirtualDirectory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StoredFile",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "VirtualDirectory");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StoredFile");

            migrationBuilder.AddColumn<bool>(
                name: "State",
                table: "VirtualDirectory",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "State",
                table: "StoredFile",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }
    }
}
