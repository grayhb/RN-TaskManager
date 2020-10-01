using Microsoft.EntityFrameworkCore.Migrations;

namespace RN_TaskManager.DAL.Migrations
{
    public partial class AddEffectFieldsInProjectTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlockName",
                table: "ProjectTasks",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EffectAfterHours",
                table: "ProjectTasks",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EffectBeforeHours",
                table: "ProjectTasks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockName",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "EffectAfterHours",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "EffectBeforeHours",
                table: "ProjectTasks");
        }
    }
}
