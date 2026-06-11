using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.Infrastructure.Vcs.Migrations.Snapshots
{
    /// <inheritdoc />
    public partial class merge_requests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "merge_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    TargetRefName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SourceRefName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MergeCommitId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_merge_requests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_merge_requests_ProjectId",
                table: "merge_requests",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "merge_requests");
        }
    }
}
