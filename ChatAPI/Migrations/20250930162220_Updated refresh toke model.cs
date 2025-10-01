using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatAPI.Migrations
{
    /// <inheritdoc />
    public partial class Updatedrefreshtokemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TokenHash",
                table: "RefreshTokens",
                newName: "Refresh_Token_Hash");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId_TokenHash",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId_Refresh_Token_Hash");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_Refresh_Token_Hash");

            migrationBuilder.AddColumn<string>(
                name: "JWT_Token",
                table: "RefreshTokens",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JWT_Token",
                table: "RefreshTokens");

            migrationBuilder.RenameColumn(
                name: "Refresh_Token_Hash",
                table: "RefreshTokens",
                newName: "TokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId_Refresh_Token_Hash",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId_TokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_Refresh_Token_Hash",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_TokenHash");
        }
    }
}
