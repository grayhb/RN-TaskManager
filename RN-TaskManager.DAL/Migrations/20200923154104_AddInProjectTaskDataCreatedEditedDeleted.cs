using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RN_TaskManager.DAL.Migrations
{
    public partial class AddInProjectTaskDataCreatedEditedDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "ProjectTasks",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "ProjectTasks",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEdited",
                table: "ProjectTasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoginCreated",
                table: "ProjectTasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoginDeleted",
                table: "ProjectTasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoginEdited",
                table: "ProjectTasks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "DateEdited",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "LoginCreated",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "LoginDeleted",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "LoginEdited",
                table: "ProjectTasks");
        }
    }
}
