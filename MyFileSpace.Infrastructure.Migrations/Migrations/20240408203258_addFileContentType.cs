using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFileSpace.Infrastructure.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class addFileContentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "StoredFile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "StoredFile");
        }
    }
}
