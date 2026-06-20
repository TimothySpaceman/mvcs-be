using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.Infrastructure.App.Migrations.Snapshots
{
    /// <inheritdoc />
    public partial class sessions_to_users_fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_sessions_UserId",
                table: "sessions");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_UserId",
                table: "sessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_sessions_UserId",
                table: "sessions");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_UserId",
                table: "sessions",
                column: "UserId",
                unique: true);
        }
    }
}
