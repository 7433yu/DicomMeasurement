using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DicomMeasurementApi.Migrations
{
    /// <inheritdoc />
    public partial class firstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Measurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    FileId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    StudyInstanceUid = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SeriesInstanceUid = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SopInstanceUid = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    FrameNumber = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    MeasurementType = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Label = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Value = table.Column<double>(type: "float", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "#FF0000"),
                    Visible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MeasurementData = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "{}"),
                    Unit = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false, defaultValue: "mm"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measurements", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Measurement_Key",
                table: "Measurements",
                columns: new[] { "UserId", "FileId", "StudyInstanceUid", "SeriesInstanceUid", "SopInstanceUid", "FrameNumber" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Measurements");
        }
    }
}
