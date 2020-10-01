using Microsoft.EntityFrameworkCore.Migrations;

namespace RN_TaskManager.DAL.Migrations
{
    public partial class CreateTaskTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaskTypeId",
                table: "ProjectTasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectDescription",
                table: "Projects",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TaskTypes",
                columns: table => new
                {
                    TaskTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskTypeName = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTypes", x => x.TaskTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_TaskTypeId",
                table: "ProjectTasks",
                column: "TaskTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_TaskTypes_TaskTypeId",
                table: "ProjectTasks",
                column: "TaskTypeId",
                principalTable: "TaskTypes",
                principalColumn: "TaskTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_TaskTypes_TaskTypeId",
                table: "ProjectTasks");

            migrationBuilder.DropTable(
                name: "TaskTypes");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTasks_TaskTypeId",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "TaskTypeId",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "ProjectDescription",
                table: "Projects");
        }
    }
}
