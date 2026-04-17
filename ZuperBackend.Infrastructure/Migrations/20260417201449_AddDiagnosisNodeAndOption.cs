using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZuperBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDiagnosisNodeAndOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiagnosisNodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NodeCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HelpText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagnosisNodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiagnosisOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiagnosisNodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NextNodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActionCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActionDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsFinalAction = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagnosisOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiagnosisOptions_DiagnosisNodes_DiagnosisNodeId",
                        column: x => x.DiagnosisNodeId,
                        principalTable: "DiagnosisNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiagnosisOptions_DiagnosisNodes_NextNodeId",
                        column: x => x.NextNodeId,
                        principalTable: "DiagnosisNodes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosisNodes_NodeCode",
                table: "DiagnosisNodes",
                column: "NodeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosisOptions_DiagnosisNodeId_DisplayOrder",
                table: "DiagnosisOptions",
                columns: new[] { "DiagnosisNodeId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosisOptions_NextNodeId",
                table: "DiagnosisOptions",
                column: "NextNodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiagnosisOptions");

            migrationBuilder.DropTable(
                name: "DiagnosisNodes");
        }
    }
}
