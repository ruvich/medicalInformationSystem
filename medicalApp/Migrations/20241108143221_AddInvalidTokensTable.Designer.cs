﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using medicalApp.Data;

#nullable disable

namespace medicalApp.Migrations
{
    [DbContext(typeof(AppDataContext))]
    [Migration("20241108143221_AddInvalidTokensTable")]
    partial class AddInvalidTokensTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("medicalApp.Models.CommentDataBaseModel", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("authorId")
                        .HasColumnType("uuid");

                    b.Property<string>("authorName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("consultationId")
                        .HasColumnType("uuid");

                    b.Property<string>("content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("modifyTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("parentId")
                        .HasColumnType("uuid");

                    b.HasKey("id");

                    b.HasIndex("authorId");

                    b.HasIndex("consultationId");

                    b.ToTable("CommentDataBaseModels");
                });

            modelBuilder.Entity("medicalApp.Models.ConsultationDataBaseModel", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("inspectionId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("specialityId")
                        .HasColumnType("uuid");

                    b.HasKey("id");

                    b.HasIndex("inspectionId");

                    b.HasIndex("specialityId");

                    b.ToTable("ConsultationsDataBaseModels");
                });

            modelBuilder.Entity("medicalApp.Models.DiagnosisDataBaseModel", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("description")
                        .HasColumnType("text");

                    b.Property<Guid>("icd10Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("inspectionId")
                        .HasColumnType("uuid");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("type")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.HasIndex("icd10Id");

                    b.HasIndex("inspectionId");

                    b.ToTable("DiagnosisDataBaseModels");
                });

            modelBuilder.Entity("medicalApp.Models.DoctorModel", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("birthday")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("gender")
                        .HasColumnType("integer");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("passwordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("specialityId")
                        .HasColumnType("uuid");

                    b.HasKey("id");

                    b.HasIndex("specialityId");

                    b.ToTable("DoctorModels");
                });

            modelBuilder.Entity("medicalApp.Models.Icd10DataBaseModel", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("code")
                        .HasColumnType("text");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<Guid?>("parentId")
                        .HasColumnType("uuid");

                    b.HasKey("id");

                    b.ToTable("Icd10DataBaseModels");
                });

            modelBuilder.Entity("medicalApp.Models.InspectionDataBaseModel", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("anamnesis")
                        .HasColumnType("text");

                    b.Property<Guid?>("baseInspectionId")
                        .HasColumnType("uuid");

                    b.Property<string>("complaints")
                        .HasColumnType("text");

                    b.Property<int>("conclusion")
                        .HasColumnType("integer");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("deathDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("doctorID")
                        .HasColumnType("uuid");

                    b.Property<bool>("hasChain")
                        .HasColumnType("boolean");

                    b.Property<bool>("hasNested")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("nextVisitDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("patientID")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("previousInspectionId")
                        .HasColumnType("uuid");

                    b.Property<string>("treatment")
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.HasIndex("doctorID");

                    b.HasIndex("patientID");

                    b.ToTable("InspectionDataBaseModels");
                });

            modelBuilder.Entity("medicalApp.Models.InvalidToken", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("finish")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("InvalidTokens");
                });

            modelBuilder.Entity("medicalApp.Models.PatientModel", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("birthday")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("gender")
                        .HasColumnType("integer");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("PatientModels");
                });

            modelBuilder.Entity("medicalApp.Models.SpecialityModel", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("SpecialityModels");
                });

            modelBuilder.Entity("medicalApp.Models.CommentDataBaseModel", b =>
                {
                    b.HasOne("medicalApp.Models.DoctorModel", "author")
                        .WithMany("сomments")
                        .HasForeignKey("authorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("medicalApp.Models.ConsultationDataBaseModel", "consultation")
                        .WithMany("comments")
                        .HasForeignKey("consultationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("author");

                    b.Navigation("consultation");
                });

            modelBuilder.Entity("medicalApp.Models.ConsultationDataBaseModel", b =>
                {
                    b.HasOne("medicalApp.Models.InspectionDataBaseModel", "inspection")
                        .WithMany("consultations")
                        .HasForeignKey("inspectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("medicalApp.Models.SpecialityModel", "speciality")
                        .WithMany("consultations")
                        .HasForeignKey("specialityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("inspection");

                    b.Navigation("speciality");
                });

            modelBuilder.Entity("medicalApp.Models.DiagnosisDataBaseModel", b =>
                {
                    b.HasOne("medicalApp.Models.Icd10DataBaseModel", "icd10")
                        .WithMany("diagnoses")
                        .HasForeignKey("icd10Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("medicalApp.Models.InspectionDataBaseModel", "inspection")
                        .WithMany("diagnoses")
                        .HasForeignKey("inspectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("icd10");

                    b.Navigation("inspection");
                });

            modelBuilder.Entity("medicalApp.Models.DoctorModel", b =>
                {
                    b.HasOne("medicalApp.Models.SpecialityModel", "speciality")
                        .WithMany("doctors")
                        .HasForeignKey("specialityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("speciality");
                });

            modelBuilder.Entity("medicalApp.Models.InspectionDataBaseModel", b =>
                {
                    b.HasOne("medicalApp.Models.DoctorModel", "doctor")
                        .WithMany("inspections")
                        .HasForeignKey("doctorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("medicalApp.Models.PatientModel", "patient")
                        .WithMany("inspections")
                        .HasForeignKey("patientID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("doctor");

                    b.Navigation("patient");
                });

            modelBuilder.Entity("medicalApp.Models.ConsultationDataBaseModel", b =>
                {
                    b.Navigation("comments");
                });

            modelBuilder.Entity("medicalApp.Models.DoctorModel", b =>
                {
                    b.Navigation("inspections");

                    b.Navigation("сomments");
                });

            modelBuilder.Entity("medicalApp.Models.Icd10DataBaseModel", b =>
                {
                    b.Navigation("diagnoses");
                });

            modelBuilder.Entity("medicalApp.Models.InspectionDataBaseModel", b =>
                {
                    b.Navigation("consultations");

                    b.Navigation("diagnoses");
                });

            modelBuilder.Entity("medicalApp.Models.PatientModel", b =>
                {
                    b.Navigation("inspections");
                });

            modelBuilder.Entity("medicalApp.Models.SpecialityModel", b =>
                {
                    b.Navigation("consultations");

                    b.Navigation("doctors");
                });
#pragma warning restore 612, 618
        }
    }
}
