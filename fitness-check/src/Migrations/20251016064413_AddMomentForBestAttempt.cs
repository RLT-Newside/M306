using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessCheck.Migrations
{
    /// <inheritdoc />
    public partial class AddMomentForBestAttempt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Discipline",
                table: "BestAttempts",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "MomentUtc",
                table: "BestAttempts",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_BestAttempts_UserId_CohortId_Discipline",
                table: "BestAttempts",
                columns: new[] { "UserId", "CohortId", "Discipline" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BestAttempts_UserId_CohortId_Discipline",
                table: "BestAttempts");

            migrationBuilder.DropColumn(
                name: "MomentUtc",
                table: "BestAttempts");

            migrationBuilder.AlterColumn<string>(
                name: "Discipline",
                table: "BestAttempts",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
