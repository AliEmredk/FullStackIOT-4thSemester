using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "app_users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "turbines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TurbineId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TurbineName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    FarmId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_turbines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "alert_events",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TurbineIdFk = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alert_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alert_events_turbines_TurbineIdFk",
                        column: x => x.TurbineIdFk,
                        principalTable: "turbines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "telemetry_readings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TurbineIdFk = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WindDirection = table.Column<double>(type: "double precision", nullable: false),
                    AmbientTemperature = table.Column<double>(type: "double precision", nullable: false),
                    RotorSpeed = table.Column<double>(type: "double precision", nullable: false),
                    PowerOutput = table.Column<double>(type: "double precision", nullable: false),
                    NacelleDirection = table.Column<double>(type: "double precision", nullable: false),
                    BladePitch = table.Column<double>(type: "double precision", nullable: false),
                    GeneratorTemp = table.Column<double>(type: "double precision", nullable: false),
                    GearboxTemp = table.Column<double>(type: "double precision", nullable: false),
                    Vibration = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_telemetry_readings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_telemetry_readings_turbines_TurbineIdFk",
                        column: x => x.TurbineIdFk,
                        principalTable: "turbines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "turbine_commands",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TurbineIdFk = table.Column<int>(type: "integer", nullable: false),
                    UserIdFk = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    PayloadJson = table.Column<string>(type: "text", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false),
                    PublishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_turbine_commands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_turbine_commands_app_users_UserIdFk",
                        column: x => x.UserIdFk,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_turbine_commands_turbines_TurbineIdFk",
                        column: x => x.TurbineIdFk,
                        principalTable: "turbines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alert_events_TurbineIdFk_Timestamp",
                table: "alert_events",
                columns: new[] { "TurbineIdFk", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_app_users_Username",
                table: "app_users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_telemetry_readings_TurbineIdFk_Timestamp",
                table: "telemetry_readings",
                columns: new[] { "TurbineIdFk", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_turbine_commands_TurbineIdFk_Timestamp",
                table: "turbine_commands",
                columns: new[] { "TurbineIdFk", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_turbine_commands_UserIdFk_Timestamp",
                table: "turbine_commands",
                columns: new[] { "UserIdFk", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_turbines_FarmId_TurbineId",
                table: "turbines",
                columns: new[] { "FarmId", "TurbineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_turbines_TurbineId",
                table: "turbines",
                column: "TurbineId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alert_events");

            migrationBuilder.DropTable(
                name: "telemetry_readings");

            migrationBuilder.DropTable(
                name: "turbine_commands");

            migrationBuilder.DropTable(
                name: "app_users");

            migrationBuilder.DropTable(
                name: "turbines");
        }
    }
}
