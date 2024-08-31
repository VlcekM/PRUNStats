﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PRUNStatsCommon;

#nullable disable

namespace PRUNStatsCommon.Migrations
{
    [DbContext(typeof(StatsContext))]
    [Migration("20240831030542_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PRUNStatsCommon.Bases.BaseModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FirstImportedAtUTC")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdatedAtUTC")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("PRGUID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("PlanetId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("PlanetId");

                    b.ToTable("Bases", "prun");
                });

            modelBuilder.Entity("PRUNStatsCommon.Companies.CompanyModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CompanyCode")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("nvarchar(4)");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("CorporationId")
                        .HasColumnType("int");

                    b.Property<int?>("Faction")
                        .HasColumnType("int");

                    b.Property<DateTime>("FirstImportedAtUTC")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdatedAtUTC")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("PRGUID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CorporationId");

                    b.HasIndex("UserId");

                    b.ToTable("Companies", "prun");
                });

            modelBuilder.Entity("PRUNStatsCommon.Corporations.CorporationModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CorporationCode")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("nvarchar(4)");

                    b.Property<string>("CorporationName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("FirstImportedAtUTC")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdatedAtUTC")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("PRGUID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Corporations", "prun");
                });

            modelBuilder.Entity("PRUNStatsCommon.Planets.PlanetModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("FirstImportedAtUTC")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdatedAtUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NaturalId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<Guid?>("PRGUID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Planets", "prun");
                });

            modelBuilder.Entity("PRUNStatsCommon.Users.UserModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("FirstImportedAtUTC")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdatedAtUTC")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("PRGUID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Users", "prun");
                });

            modelBuilder.Entity("PRUNStatsCommon.Bases.BaseModel", b =>
                {
                    b.HasOne("PRUNStatsCommon.Companies.CompanyModel", "Company")
                        .WithMany("Bases")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PRUNStatsCommon.Planets.PlanetModel", "Planet")
                        .WithMany("Bases")
                        .HasForeignKey("PlanetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");

                    b.Navigation("Planet");
                });

            modelBuilder.Entity("PRUNStatsCommon.Companies.CompanyModel", b =>
                {
                    b.HasOne("PRUNStatsCommon.Corporations.CorporationModel", "Corporation")
                        .WithMany("Companies")
                        .HasForeignKey("CorporationId");

                    b.HasOne("PRUNStatsCommon.Users.UserModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Corporation");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PRUNStatsCommon.Companies.CompanyModel", b =>
                {
                    b.Navigation("Bases");
                });

            modelBuilder.Entity("PRUNStatsCommon.Corporations.CorporationModel", b =>
                {
                    b.Navigation("Companies");
                });

            modelBuilder.Entity("PRUNStatsCommon.Planets.PlanetModel", b =>
                {
                    b.Navigation("Bases");
                });
#pragma warning restore 612, 618
        }
    }
}
