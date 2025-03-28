using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace medicalApp.Migrations
{
    /// <inheritdoc />
    public partial class AddChainAndNestedFieldsToInspection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "hasChain",
                table: "InspectionDataBaseModels",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "hasNested",
                table: "InspectionDataBaseModels",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hasChain",
                table: "InspectionDataBaseModels");

            migrationBuilder.DropColumn(
                name: "hasNested",
                table: "InspectionDataBaseModels");
        }
    }
}
