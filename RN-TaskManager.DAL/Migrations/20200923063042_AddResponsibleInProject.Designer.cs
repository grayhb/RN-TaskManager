﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RN_TaskManager.DAL.Context;

namespace RN_TaskManager.DAL.Migrations
{
    [DbContext(typeof(RN_TaskManagerContext))]
    [Migration("20200923063042_AddResponsibleInProject")]
    partial class AddResponsibleInProject
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("RN_TaskManager.Models.Group", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<string>("GroupName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GroupNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GroupId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("RN_TaskManager.Models.Project", b =>
                {
                    b.Property<int>("ProjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<int>("ProjectImportance")
                        .HasColumnType("int");

                    b.Property<string>("ProjectName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("RN_TaskManager.Models.ProjectTask", b =>
                {
                    b.Property<int>("ProjectTaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<string>("Details")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DurationHours")
                        .HasColumnType("int");

                    b.Property<DateTime?>("EndFact")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("EndPlan")
                        .HasColumnType("datetime2");

                    b.Property<int?>("GroupId")
                        .HasColumnType("int");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int?>("ProjectTaskStatusId")
                        .HasColumnType("int");

                    b.Property<int?>("ProjectTaskTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("StartFact")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("StartPlan")
                        .HasColumnType("datetime2");

                    b.HasKey("ProjectTaskId");

                    b.HasIndex("GroupId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("ProjectTaskStatusId");

                    b.HasIndex("ProjectTaskTypeId");

                    b.ToTable("ProjectTasks");
                });

            modelBuilder.Entity("RN_TaskManager.Models.ProjectTaskPerformer", b =>
                {
                    b.Property<int>("ProjectTaskPerformerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<int>("ProjectTaskId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ProjectTaskPerformerId");

                    b.HasIndex("ProjectTaskId");

                    b.HasIndex("UserId");

                    b.ToTable("ProjectTaskPerformers");
                });

            modelBuilder.Entity("RN_TaskManager.Models.ProjectTaskStatus", b =>
                {
                    b.Property<int>("ProjectTaskStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<string>("StatusColor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StatusName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProjectTaskStatusId");

                    b.ToTable("Statuses");
                });

            modelBuilder.Entity("RN_TaskManager.Models.ProjectTaskType", b =>
                {
                    b.Property<int>("ProjectTaskTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<string>("ProjectTaskTypeName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProjectTaskTypeId");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectTaskTypes");
                });

            modelBuilder.Entity("RN_TaskManager.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("GroupId")
                        .HasColumnType("int");

                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Login")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Patronymic")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.HasIndex("GroupId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RN_TaskManager.Models.Project", b =>
                {
                    b.HasOne("RN_TaskManager.Models.User", "Responsible")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("RN_TaskManager.Models.ProjectTask", b =>
                {
                    b.HasOne("RN_TaskManager.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");

                    b.HasOne("RN_TaskManager.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RN_TaskManager.Models.ProjectTaskStatus", "TaskStatus")
                        .WithMany()
                        .HasForeignKey("ProjectTaskStatusId");

                    b.HasOne("RN_TaskManager.Models.ProjectTaskType", "TaskType")
                        .WithMany()
                        .HasForeignKey("ProjectTaskTypeId");
                });

            modelBuilder.Entity("RN_TaskManager.Models.ProjectTaskPerformer", b =>
                {
                    b.HasOne("RN_TaskManager.Models.ProjectTask", "Task")
                        .WithMany("ProjectTaskPerformers")
                        .HasForeignKey("ProjectTaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RN_TaskManager.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RN_TaskManager.Models.ProjectTaskType", b =>
                {
                    b.HasOne("RN_TaskManager.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RN_TaskManager.Models.User", b =>
                {
                    b.HasOne("RN_TaskManager.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");
                });
#pragma warning restore 612, 618
        }
    }
}
