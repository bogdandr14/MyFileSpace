using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFileSpace.Infrastructure.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoriteFile",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteFile", x => new { x.UserId, x.FileId });
                    table.ForeignKey(
                        name: "FK_FavoriteFile_StoredFile_FileId",
                        column: x => x.FileId,
                        principalTable: "StoredFile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteFile_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteFile_FileId",
                table: "FavoriteFile",
                column: "FileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteFile");
        }
    }
}
