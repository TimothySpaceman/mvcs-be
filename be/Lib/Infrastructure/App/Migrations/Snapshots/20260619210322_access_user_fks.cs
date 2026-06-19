using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.Infrastructure.App.Migrations.Snapshots
{
    /// <inheritdoc />
    public partial class access_user_fks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_project_tasks_users_AuthorId",
                table: "project_tasks");

            migrationBuilder.AlterColumn<Guid>(
                name: "AuthorId",
                table: "project_tasks",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_project_access_users_UserId",
                table: "project_access",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_project_tasks_users_AuthorId",
                table: "project_tasks",
                column: "AuthorId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_storage_access_users_UserId",
                table: "storage_access",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_project_access_users_UserId",
                table: "project_access");

            migrationBuilder.DropForeignKey(
                name: "FK_project_tasks_users_AuthorId",
                table: "project_tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_storage_access_users_UserId",
                table: "storage_access");

            migrationBuilder.AlterColumn<Guid>(
                name: "AuthorId",
                table: "project_tasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_project_tasks_users_AuthorId",
                table: "project_tasks",
                column: "AuthorId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
