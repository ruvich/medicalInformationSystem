using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace medicalApp.Migrations
{
    /// <inheritdoc />
    public partial class newFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentDataBaseModels_ConsultationsDataBaseModels_сonsultat",
                table: "CommentDataBaseModels");

            migrationBuilder.RenameColumn(
                name: "сonsultationId",
                table: "CommentDataBaseModels",
                newName: "consultationId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentDataBaseModels_сonsultationId",
                table: "CommentDataBaseModels",
                newName: "IX_CommentDataBaseModels_сonsultationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentDataBaseModels_ConsultationsDataBaseModels_сonsultat",
                table: "CommentDataBaseModels",
                column: "consultationId",
                principalTable: "ConsultationsDataBaseModels",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentDataBaseModels_ConsultationsDataBaseModels_сonsultat",
                table: "CommentDataBaseModels");

            migrationBuilder.RenameColumn(
                name: "сonsultationId",
                table: "CommentDataBaseModels",
                newName: "consultationId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentDataBaseModels_сonsultationId",
                table: "CommentDataBaseModels",
                newName: "IX_CommentDataBaseModels_consultationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentDataBaseModels_ConsultationsDataBaseModels_сonsultat",
                table: "CommentDataBaseModels",
                column: "consultationId",
                principalTable: "ConsultationsDataBaseModels",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
