using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.Infrastructure.Vcs.Migrations.Snapshots
{
    /// <inheritdoc />
    public partial class multibranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Kind",
                table: "commits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SecondParentId",
                table: "commits",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kind",
                table: "commits");

            migrationBuilder.DropColumn(
                name: "SecondParentId",
                table: "commits");
        }
    }
}
