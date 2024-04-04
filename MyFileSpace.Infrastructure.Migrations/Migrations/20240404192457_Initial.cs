using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFileSpace.Infrastructure.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessKey",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nchar(255)", fixedLength: true, maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessKey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(321)", maxLength: 321, nullable: false),
                    Role = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    LastPasswordChange = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    Salt = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.UniqueConstraint("AK_User_TagName", x => x.TagName);
                });

            migrationBuilder.CreateTable(
                name: "Label",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Label", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Label_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VirtualDirectory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentDirectoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VirtualPath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AccessLevel = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    State = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualDirectory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VirtualDirectory_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VirtualDirectory_VirtualDirectory_ParentDirectoryId",
                        column: x => x.ParentDirectoryId,
                        principalTable: "VirtualDirectory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DirectoryAccessKey",
                columns: table => new
                {
                    DirectoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessKeyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoryAccessKey", x => new { x.DirectoryId, x.AccessKeyId });
                    table.ForeignKey(
                        name: "FK_DirectoryAccessKey_AccessKey_AccessKeyId",
                        column: x => x.AccessKeyId,
                        principalTable: "AccessKey",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectoryAccessKey_VirtualDirectory_DirectoryId",
                        column: x => x.DirectoryId,
                        principalTable: "VirtualDirectory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoredFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DirectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AccessLevel = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    State = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoredFile_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoredFile_VirtualDirectory_DirectorId",
                        column: x => x.DirectorId,
                        principalTable: "VirtualDirectory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserDirectoryAccess",
                columns: table => new
                {
                    DirectoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDirectoryAccess", x => new { x.DirectoryId, x.AllowedUserId });
                    table.ForeignKey(
                        name: "FK_UserDirectoryAccess_User_AllowedUserId",
                        column: x => x.AllowedUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDirectoryAccess_VirtualDirectory_DirectoryId",
                        column: x => x.DirectoryId,
                        principalTable: "VirtualDirectory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FileAccessKey",
                columns: table => new
                {
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessKeyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAccessKey", x => new { x.FileId, x.AccessKeyId });
                    table.ForeignKey(
                        name: "FK_FileAccessKey_AccessKey_AccessKeyId",
                        column: x => x.AccessKeyId,
                        principalTable: "AccessKey",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileAccessKey_StoredFile_FileId",
                        column: x => x.FileId,
                        principalTable: "StoredFile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileLabel",
                columns: table => new
                {
                    LabelId = table.Column<int>(type: "int", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileLabel", x => new { x.LabelId, x.FileId });
                    table.ForeignKey(
                        name: "FK_FileLabel_Label_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Label",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileLabel_StoredFile_FileId",
                        column: x => x.FileId,
                        principalTable: "StoredFile",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserFileAccess",
                columns: table => new
                {
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFileAccess", x => new { x.FileId, x.AllowedUserId });
                    table.ForeignKey(
                        name: "FK_UserFileAccess_StoredFile_FileId",
                        column: x => x.FileId,
                        principalTable: "StoredFile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFileAccess_User_AllowedUserId",
                        column: x => x.AllowedUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DirectoryAccessKey_AccessKeyId",
                table: "DirectoryAccessKey",
                column: "AccessKeyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DirectoryAccessKey_DirectoryId",
                table: "DirectoryAccessKey",
                column: "DirectoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileAccessKey_AccessKeyId",
                table: "FileAccessKey",
                column: "AccessKeyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileAccessKey_FileId",
                table: "FileAccessKey",
                column: "FileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileLabel_FileId",
                table: "FileLabel",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Label_OwnerId",
                table: "Label",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_StoredFile_DirectorId",
                table: "StoredFile",
                column: "DirectorId");

            migrationBuilder.CreateIndex(
                name: "IX_StoredFile_OwnerId",
                table: "StoredFile",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_User_TagName",
                table: "User",
                column: "TagName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDirectoryAccess_AllowedUserId",
                table: "UserDirectoryAccess",
                column: "AllowedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFileAccess_AllowedUserId",
                table: "UserFileAccess",
                column: "AllowedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualDirectory_OwnerId",
                table: "VirtualDirectory",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualDirectory_ParentDirectoryId",
                table: "VirtualDirectory",
                column: "ParentDirectoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DirectoryAccessKey");

            migrationBuilder.DropTable(
                name: "FileAccessKey");

            migrationBuilder.DropTable(
                name: "FileLabel");

            migrationBuilder.DropTable(
                name: "UserDirectoryAccess");

            migrationBuilder.DropTable(
                name: "UserFileAccess");

            migrationBuilder.DropTable(
                name: "AccessKey");

            migrationBuilder.DropTable(
                name: "Label");

            migrationBuilder.DropTable(
                name: "StoredFile");

            migrationBuilder.DropTable(
                name: "VirtualDirectory");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
