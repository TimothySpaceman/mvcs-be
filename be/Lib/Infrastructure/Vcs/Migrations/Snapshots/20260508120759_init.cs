using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.Infrastructure.Vcs.Migrations.Snapshots
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "blob_metadata",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "bytea", maxLength: 16, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Length = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blob_metadata", x => new { x.Id, x.ProjectId });
                });

            migrationBuilder.CreateTable(
                name: "commits",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "bytea", maxLength: 16, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<byte[]>(type: "bytea", maxLength: 16, nullable: true),
                    Message = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    AuthorName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    AuthorEmail = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Changes = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_blob_metadata_ProjectId",
                table: "blob_metadata",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_commits_ProjectId",
                table: "commits",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blob_metadata");

            migrationBuilder.DropTable(
                name: "commits");
        }
    }
}
