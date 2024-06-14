﻿// <auto-generated />
using System;
using DB_AngoraLib.EF_DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DB_AngoraREST.Migrations
{
    [DbContext(typeof(DB_AngoraContext))]
    [Migration("20240614064354_DbAngoraMig02")]
    partial class DbAngoraMig02
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DB_AngoraLib.Models.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("RabbitEarCombId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RabbitEarCombId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("DB_AngoraLib.Models.Rabbit", b =>
                {
                    b.Property<string>("EarCombId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Color")
                        .HasColumnType("int");

                    b.Property<DateOnly>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<DateOnly?>("DateOfDeath")
                        .HasColumnType("date");

                    b.Property<string>("FatherId_Placeholder")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Father_EarCombId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("ForSale")
                        .HasColumnType("int");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("LeftEarId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MotherId_Placeholder")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Mother_EarCombId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("NickName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OriginId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("OwnerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Race")
                        .HasColumnType("int");

                    b.Property<string>("RightEarId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EarCombId");

                    b.HasIndex("Father_EarCombId");

                    b.HasIndex("Mother_EarCombId");

                    b.HasIndex("OriginId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Rabbits");
                });

            modelBuilder.Entity("DB_AngoraLib.Models.Rating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BodyNotice")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("BodyPoint")
                        .HasColumnType("int");

                    b.Property<DateOnly>("DateRated")
                        .HasColumnType("date");

                    b.Property<string>("FurNotice")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FurPoint")
                        .HasColumnType("int");

                    b.Property<string>("LeftEarId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RabbitEarCombId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RightEarId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TotalPoint")
                        .HasColumnType("int");

                    b.Property<string>("WeightNotice")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WeightPoint")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RabbitEarCombId");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("DB_AngoraLib.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("BreederRegNo")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("RoadNameAndNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("ZipCode")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BreederRegNo")
                        .IsUnique()
                        .HasFilter("[BreederRegNo] IS NOT NULL");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("DB_AngoraLib.Models.Photo", b =>
                {
                    b.HasOne("DB_AngoraLib.Models.Rabbit", null)
                        .WithMany("Photos")
                        .HasForeignKey("RabbitEarCombId");
                });

            modelBuilder.Entity("DB_AngoraLib.Models.Rabbit", b =>
                {
                    b.HasOne("DB_AngoraLib.Models.Rabbit", "Father")
                        .WithMany("FatheredChildren")
                        .HasForeignKey("Father_EarCombId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DB_AngoraLib.Models.Rabbit", "Mother")
                        .WithMany("MotheredChildren")
                        .HasForeignKey("Mother_EarCombId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DB_AngoraLib.Models.User", "UserOrigin")
                        .WithMany("RabbitsLinked")
                        .HasForeignKey("OriginId");

                    b.HasOne("DB_AngoraLib.Models.User", "UserOwner")
                        .WithMany("RabbitsOwned")
                        .HasForeignKey("OwnerId");

                    b.Navigation("Father");

                    b.Navigation("Mother");

                    b.Navigation("UserOrigin");

                    b.Navigation("UserOwner");
                });

            modelBuilder.Entity("DB_AngoraLib.Models.Rating", b =>
                {
                    b.HasOne("DB_AngoraLib.Models.Rabbit", "Rabbit")
                        .WithMany()
                        .HasForeignKey("RabbitEarCombId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Rabbit");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("DB_AngoraLib.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("DB_AngoraLib.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DB_AngoraLib.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("DB_AngoraLib.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DB_AngoraLib.Models.Rabbit", b =>
                {
                    b.Navigation("FatheredChildren");

                    b.Navigation("MotheredChildren");

                    b.Navigation("Photos");
                });

            modelBuilder.Entity("DB_AngoraLib.Models.User", b =>
                {
                    b.Navigation("RabbitsLinked");

                    b.Navigation("RabbitsOwned");
                });
#pragma warning restore 612, 618
        }
    }
}
