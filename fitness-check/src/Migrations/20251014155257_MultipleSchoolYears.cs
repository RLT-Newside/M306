using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessCheck.Migrations
{
    /// <inheritdoc />
    public partial class MultipleSchoolYears : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "TwelveMinutesRunAttempts");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "StandingLongJumpAttempts");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "ShuttleRunAttempts");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "OneLegStandAttempts");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "MedicineBallPushAttempts");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "CoreStrengthAttempts");

            migrationBuilder.CreateTable(
                name: "BestAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CohortId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Discipline = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Result = table.Column<float>(type: "float", nullable: false),
                    Points = table.Column<uint>(type: "int unsigned", nullable: false),
                    LeftFootResult = table.Column<uint>(type: "int unsigned", nullable: true),
                    RightFootResult = table.Column<uint>(type: "int unsigned", nullable: true),
                    LastCalculated = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BestAttempts", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BestAttempts");

            migrationBuilder.AddColumn<uint>(
                name: "Points",
                table: "TwelveMinutesRunAttempts",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "Points",
                table: "StandingLongJumpAttempts",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "Points",
                table: "ShuttleRunAttempts",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "Points",
                table: "OneLegStandAttempts",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "Points",
                table: "MedicineBallPushAttempts",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "Points",
                table: "CoreStrengthAttempts",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u);
        }
    }
}
