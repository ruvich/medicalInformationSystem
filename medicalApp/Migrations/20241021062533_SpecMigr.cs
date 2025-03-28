using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace medicalApp.Migrations
{
    /// <inheritdoc />
    public partial class SpecMigr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "speciality",
                table: "DoctorModels",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "SpecialityModels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialityModels", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecialityModels");

            migrationBuilder.DropColumn(
                name: "speciality",
                table: "DoctorModels");
        }
    }
}
