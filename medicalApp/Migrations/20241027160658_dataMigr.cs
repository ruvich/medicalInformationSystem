using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace medicalApp.Migrations
{
    /// <inheritdoc />
    public partial class dataMigr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "speciality",
                table: "DoctorModels",
                newName: "specialityId");

            migrationBuilder.CreateTable(
                name: "InspectionDataBaseModels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    anamnesis = table.Column<string>(type: "text", nullable: true),
                    complaints = table.Column<string>(type: "text", nullable: true),
                    treatment = table.Column<string>(type: "text", nullable: true),
                    conclusion = table.Column<int>(type: "integer", nullable: false),
                    nextVisitDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deathDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    baseInspectionId = table.Column<Guid>(type: "uuid", nullable: true),
                    previousInspectionId = table.Column<Guid>(type: "uuid", nullable: true),
                    patientID = table.Column<Guid>(type: "uuid", nullable: false),
                    doctorID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionDataBaseModels", x => x.id);
                    table.ForeignKey(
                        name: "FK_InspectionDataBaseModels_DoctorModels_doctorID",
                        column: x => x.doctorID,
                        principalTable: "DoctorModels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InspectionDataBaseModels_PatientModels_patientID",
                        column: x => x.patientID,
                        principalTable: "PatientModels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsultationsDataBaseModels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    inspectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    specialityId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationsDataBaseModels", x => x.id);
                    table.ForeignKey(
                        name: "FK_ConsultationsDataBaseModels_InspectionDataBaseModels_inspec~",
                        column: x => x.inspectionId,
                        principalTable: "InspectionDataBaseModels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsultationsDataBaseModels_SpecialityModels_specialityId",
                        column: x => x.specialityId,
                        principalTable: "SpecialityModels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiagnosisDataBaseModels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    inspectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    icd10Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagnosisDataBaseModels", x => x.id);
                    table.ForeignKey(
                        name: "FK_DiagnosisDataBaseModels_Icd10DataBaseModels_icd10Id",
                        column: x => x.icd10Id,
                        principalTable: "Icd10DataBaseModels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiagnosisDataBaseModels_InspectionDataBaseModels_inspection~",
                        column: x => x.inspectionId,
                        principalTable: "InspectionDataBaseModels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentDataBaseModels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modifyTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    content = table.Column<string>(type: "text", nullable: false),
                    authorId = table.Column<Guid>(type: "uuid", nullable: false),
                    authorName = table.Column<string>(type: "text", nullable: false),
                    parentId = table.Column<Guid>(type: "uuid", nullable: true),
                    сonsultationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentDataBaseModels", x => x.id);
                    table.ForeignKey(
                        name: "FK_CommentDataBaseModels_ConsultationsDataBaseModels_сonsultat~",
                        column: x => x.сonsultationId,
                        principalTable: "ConsultationsDataBaseModels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentDataBaseModels_DoctorModels_authorId",
                        column: x => x.authorId,
                        principalTable: "DoctorModels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorModels_specialityId",
                table: "DoctorModels",
                column: "specialityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentDataBaseModels_сonsultationId",
                table: "CommentDataBaseModels",
                column: "сonsultationId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentDataBaseModels_authorId",
                table: "CommentDataBaseModels",
                column: "authorId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationsDataBaseModels_inspectionId",
                table: "ConsultationsDataBaseModels",
                column: "inspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationsDataBaseModels_specialityId",
                table: "ConsultationsDataBaseModels",
                column: "specialityId");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosisDataBaseModels_icd10Id",
                table: "DiagnosisDataBaseModels",
                column: "icd10Id");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosisDataBaseModels_inspectionId",
                table: "DiagnosisDataBaseModels",
                column: "inspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionDataBaseModels_doctorID",
                table: "InspectionDataBaseModels",
                column: "doctorID");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionDataBaseModels_patientID",
                table: "InspectionDataBaseModels",
                column: "patientID");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorModels_SpecialityModels_specialityId",
                table: "DoctorModels",
                column: "specialityId",
                principalTable: "SpecialityModels",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorModels_SpecialityModels_specialityId",
                table: "DoctorModels");

            migrationBuilder.DropTable(
                name: "CommentDataBaseModels");

            migrationBuilder.DropTable(
                name: "DiagnosisDataBaseModels");

            migrationBuilder.DropTable(
                name: "ConsultationsDataBaseModels");

            migrationBuilder.DropTable(
                name: "InspectionDataBaseModels");

            migrationBuilder.DropIndex(
                name: "IX_DoctorModels_specialityId",
                table: "DoctorModels");

            migrationBuilder.RenameColumn(
                name: "specialityId",
                table: "DoctorModels",
                newName: "speciality");
        }
    }
}
