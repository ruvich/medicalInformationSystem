using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using medicalApp.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace medicalApp.Data
{
    public class AppDataContext: DbContext
    {
        public AppDataContext(DbContextOptions<AppDataContext> options) : base(options) {
            //Database.Migrate();
        }
        public DbSet<DoctorModel> DoctorModels { get; set; }
        public DbSet<SpecialityModel> SpecialityModels { get; set; }
        public DbSet<PatientModel> PatientModels { get; set; }
        public DbSet<Icd10DataBaseModel> Icd10DataBaseModels { get; set; }
        public DbSet<InspectionDataBaseModel> InspectionDataBaseModels { get; set; }
        public DbSet<DiagnosisDataBaseModel> DiagnosisDataBaseModels { get; set; }
        public DbSet<ConsultationDataBaseModel> ConsultationsDataBaseModels { get; set; }
        public DbSet<CommentDataBaseModel> CommentDataBaseModels { get; set; }
        public DbSet<InvalidToken> InvalidTokens { get; set; }
    }
}
