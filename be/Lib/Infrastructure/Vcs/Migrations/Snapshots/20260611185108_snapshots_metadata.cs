using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.Infrastructure.Vcs.Migrations.Snapshots
{
    /// <inheritdoc />
    public partial class snapshots_metadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "snapshot_metadata",
                columns: table => new
                {
                    CommitId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<string>(type: "jsonb", nullable: false),
                    SubmittedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_snapshot_metadata", x => new { x.CommitId, x.ProjectId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_snapshot_metadata_Data",
                table: "snapshot_metadata",
                column: "Data")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_snapshot_metadata_ProjectId",
                table: "snapshot_metadata",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "snapshot_metadata");
        }
    }
}
