using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.Infrastructure.App.Migrations.Snapshots
{
    /// <inheritdoc />
    public partial class tasks_basics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "project_tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CommitId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Deadline = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_project_tasks_projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_project_tasks_users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "task_assignments",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_assignments", x => new { x.TaskId, x.UserId });
                    table.ForeignKey(
                        name: "FK_task_assignments_project_tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "project_tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_task_assignments_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_project_tasks_AuthorId",
                table: "project_tasks",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_project_tasks_ProjectId",
                table: "project_tasks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_project_tasks_Status",
                table: "project_tasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_task_assignments_UserId",
                table: "task_assignments",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "task_assignments");

            migrationBuilder.DropTable(
                name: "project_tasks");
        }
    }
}
