using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.Infrastructure.Vcs.Migrations.Snapshots
{
    /// <inheritdoc />
    public partial class commits_composite_pk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_commits",
                table: "commits");

            migrationBuilder.AddPrimaryKey(
                name: "PK_commits",
                table: "commits",
                columns: new[] { "Id", "ProjectId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_commits",
                table: "commits");

            migrationBuilder.AddPrimaryKey(
                name: "PK_commits",
                table: "commits",
                column: "Id");
        }
    }
}
