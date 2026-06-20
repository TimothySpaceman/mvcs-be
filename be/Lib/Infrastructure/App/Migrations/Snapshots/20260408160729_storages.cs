using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.Infrastructure.App.Migrations.Snapshots
{
    /// <inheritdoc />
    public partial class storages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "storage_types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Label = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ConfigSchema = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_storage_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "storages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    StorageTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Config = table.Column<string>(type: "jsonb", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_storages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_storages_storage_types_StorageTypeId",
                        column: x => x.StorageTypeId,
                        principalTable: "storage_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "storage_access",
                columns: table => new
                {
                    StorageId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_storage_access", x => new { x.StorageId, x.UserId });
                    table.ForeignKey(
                        name: "FK_storage_access_storages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_storage_access_UserId",
                table: "storage_access",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_storage_types_Key",
                table: "storage_types",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_storage_types_Label",
                table: "storage_types",
                column: "Label");

            migrationBuilder.CreateIndex(
                name: "IX_storages_StorageTypeId",
                table: "storages",
                column: "StorageTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "storage_access");

            migrationBuilder.DropTable(
                name: "storages");

            migrationBuilder.DropTable(
                name: "storage_types");
        }
    }
}
