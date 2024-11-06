﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using QueueManagementSystem.MVC.Data;

#nullable disable

namespace QueueManagementSystem.MVC.Migrations
{
    [DbContext(typeof(QueueManagementSystemContext))]
    [Migration("20241104053057_createNewDb")]
    partial class createNewDb
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.Advert", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Duration")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Adverts");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.Biometrics", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("FingerPrintData")
                        .HasColumnType("bytea");

                    b.Property<string>("IdNumber")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Biodata");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.CompanyInformation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Logo")
                        .HasColumnType("text");

                    b.Property<string>("PhysicalAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ReportFooterMessage")
                        .HasColumnType("text");

                    b.Property<string>("Website")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("CompanyInformation");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.Configurations.Configuration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool?>("BoolValue")
                        .HasColumnType("boolean");

                    b.Property<string>("ConfigurationName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("IntValue")
                        .HasColumnType("integer");

                    b.Property<string>("StringValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ValueType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.Feedback.Feedback", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Challenges")
                        .HasColumnType("integer");

                    b.Property<string>("ImprovementSuggestion")
                        .HasColumnType("text");

                    b.Property<DateTime>("SubmittedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("WaitTime")
                        .HasColumnType("integer");

                    b.Property<int>("WaitTimeAcceptance")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.NotificationMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsRead")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TicketNo")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.Privilege", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("createdAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Privileges");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.RolePrivilege", b =>
                {
                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<int>("PrivilegeId")
                        .HasColumnType("integer");

                    b.HasKey("RoleId", "PrivilegeId");

                    b.HasIndex("PrivilegeId");

                    b.ToTable("RolePrivileges");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.ServedTicket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CustomerName")
                        .HasColumnType("text");

                    b.Property<string>("CustomerPhoneNumber")
                        .HasColumnType("text");

                    b.Property<DateTime>("FinishTime")
                        .HasColumnType("timestamp");

                    b.Property<string>("IdNumber")
                        .HasColumnType("text");

                    b.Property<DateTime>("PrintTime")
                        .HasColumnType("timestamp");

                    b.Property<int>("ServiceId")
                        .HasColumnType("integer");

                    b.Property<string>("ServiceName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ServicePointAssignmentTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("ServicePointId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ShowUpTime")
                        .HasColumnType("timestamp");

                    b.Property<string>("TicketNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ServicePointId");

                    b.ToTable("ServedTicket", (string)null);
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.Service", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ServiceCategoryId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ServiceCategoryId");

                    b.ToTable("Service", (string)null);
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.ServiceCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ServiceCategories");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.ServicePoint", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ServiceId")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.ToTable("ServicePoint", (string)null);
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.ServiceProvider", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullNames")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("ServiceId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.ToTable("ServiceProvider", (string)null);
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.ServiceProviderAssignment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AssignmentTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<int>("ServicePointId")
                        .HasColumnType("integer");

                    b.Property<int>("ServiceProviderId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("SignInTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("SignOutTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ServicePointId");

                    b.HasIndex("ServiceProviderId");

                    b.ToTable("ServiceProviderAssignments");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.Sms", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<string>("MobileNo")
                        .HasColumnType("text");

                    b.Property<bool>("SentStatus")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("SmsMessages");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.SmsConfig.SmsConfigDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Classification")
                        .HasColumnType("integer");

                    b.Property<string>("Key")
                        .HasColumnType("text");

                    b.Property<int>("ParameterType")
                        .HasColumnType("integer");

                    b.Property<int>("SmsSettingsModelId")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("SmsSettingsModelId");

                    b.ToTable("SmsConfigs");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.SmsConfig.SmsSettingsModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("HttpMethod")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ResponseType")
                        .HasColumnType("integer");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("SmsSettings");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.Smtp.EmailSettingsModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("EnableSsl")
                        .HasColumnType("boolean");

                    b.Property<int>("MailPort")
                        .HasColumnType("integer");

                    b.Property<string>("MailServer")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("Sender")
                        .HasColumnType("text");

                    b.Property<string>("SenderName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("SmptSettings");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("BiometricsId")
                        .HasColumnType("integer");

                    b.Property<string>("CustomerName")
                        .HasColumnType("text");

                    b.Property<string>("CustomerPhoneNumber")
                        .HasColumnType("text");

                    b.Property<string>("IdNumber")
                        .HasColumnType("text");

                    b.Property<bool?>("IsEmergency")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsLocked")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastNoShowTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("LockedByServiceProviderId")
                        .HasColumnType("text");

                    b.Property<DateTime>("PrintTime")
                        .HasColumnType("timestamp");

                    b.Property<string>("ServiceName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ServicePointAssignmentTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("ServicePointId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ShowUpTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TicketNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("WasNoShow")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("BiometricsId");

                    b.HasIndex("ServicePointId");

                    b.ToTable("QueuedTicket", (string)null);
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("createdAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.Users.SystemUsersModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<string>("OtherNames")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("SystemUsers");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.RolePrivilege", b =>
                {
                    b.HasOne("QueueManagementSystem.MVC.Models.Privilege", "Privilege")
                        .WithMany("RolePrivileges")
                        .HasForeignKey("PrivilegeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("QueueManagementSystem.MVC.Models.UserRole", "Role")
                        .WithMany("RolePrivileges")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Privilege");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.ServedTicket", b =>
                {
                    b.HasOne("QueueManagementSystem.MVC.Models.ServicePoint", "ServicePoint")
                        .WithMany()
                        .HasForeignKey("ServicePointId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ServicePoint");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.Service", b =>
                {
                    b.HasOne("QueueManagementSystem.MVC.Models.ServiceCategory", "ServiceCategory")
                        .WithMany("Services")
                        .HasForeignKey("ServiceCategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ServiceCategory");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.ServicePoint", b =>
                {
                    b.HasOne("QueueManagementSystem.MVC.Models.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.ServiceProvider", b =>
                {
                    b.HasOne("QueueManagementSystem.MVC.Models.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Service");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.ServiceProviderAssignment", b =>
                {
                    b.HasOne("QueueManagementSystem.MVC.Models.ServicePoint", "ServicePoint")
                        .WithMany()
                        .HasForeignKey("ServicePointId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QueueManagementSystem.MVC.Models.ServiceProvider", "ServiceProvider")
                        .WithMany()
                        .HasForeignKey("ServiceProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServicePoint");

                    b.Navigation("ServiceProvider");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.SmsConfig.SmsConfigDetails", b =>
                {
                    b.HasOne("QueueManagementSystem.MVC.Models.SmsConfig.SmsSettingsModel", "SmsSettingsModel")
                        .WithMany("SmsConfigDetails")
                        .HasForeignKey("SmsSettingsModelId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("SmsSettingsModel");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.Ticket", b =>
                {
                    b.HasOne("QueueManagementSystem.MVC.Models.Biometrics", "Biometrics")
                        .WithMany()
                        .HasForeignKey("BiometricsId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("QueueManagementSystem.MVC.Models.ServicePoint", "ServicePoint")
                        .WithMany()
                        .HasForeignKey("ServicePointId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Biometrics");

                    b.Navigation("ServicePoint");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.Privilege", b =>
                {
                    b.Navigation("RolePrivileges");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.ServiceCategory", b =>
                {
                    b.Navigation("Services");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.SmsConfig.SmsSettingsModel", b =>
                {
                    b.Navigation("SmsConfigDetails");
                });

            modelBuilder.Entity("QueueManagementSystem.MVC.Models.UserRole", b =>
                {
                    b.Navigation("RolePrivileges");
                });
#pragma warning restore 612, 618
        }
    }
}
