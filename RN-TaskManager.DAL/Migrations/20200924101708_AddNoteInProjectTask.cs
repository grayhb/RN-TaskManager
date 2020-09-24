using Microsoft.EntityFrameworkCore.Migrations;

namespace RN_TaskManager.DAL.Migrations
{
    public partial class AddNoteInProjectTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "ProjectTasks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "ProjectTasks");
        }
    }
}
