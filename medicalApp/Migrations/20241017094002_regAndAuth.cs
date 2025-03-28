using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace medicalApp.Migrations
{
    /// <inheritdoc />
    public partial class regAndAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoctorModels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    birthday = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    passwordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorModels", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorModels");
        }
    }
}
