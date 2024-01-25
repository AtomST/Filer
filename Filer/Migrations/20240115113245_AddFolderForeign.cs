using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Filer.Migrations
{
    /// <inheritdoc />
    public partial class AddFolderForeign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FolderId",
                table: "Files",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_FolderId",
                table: "Files",
                column: "FolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Folders_FolderId",
                table: "Files",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Folders_FolderId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_FolderId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "FolderId",
                table: "Files");
        }
    }
}
