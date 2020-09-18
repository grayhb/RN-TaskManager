using Microsoft.EntityFrameworkCore.Migrations;

namespace RN_TaskManager.DAL.Migrations
{
    public partial class AddFKProjectInProjectTaskType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "ProjectTaskTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskTypes_ProjectId",
                table: "ProjectTaskTypes",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTaskTypes_Projects_ProjectId",
                table: "ProjectTaskTypes",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskTypes_Projects_ProjectId",
                table: "ProjectTaskTypes");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTaskTypes_ProjectId",
                table: "ProjectTaskTypes");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ProjectTaskTypes");
        }
    }
}
