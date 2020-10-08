using Microsoft.EntityFrameworkCore.Migrations;

namespace RN_TaskManager.DAL.Migrations
{
    public partial class AddBlockInProjectTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockName",
                table: "ProjectTasks");

            migrationBuilder.AddColumn<int>(
                name: "BlockId",
                table: "ProjectTasks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_BlockId",
                table: "ProjectTasks",
                column: "BlockId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_Blocks_BlockId",
                table: "ProjectTasks",
                column: "BlockId",
                principalTable: "Blocks",
                principalColumn: "BlockId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_Blocks_BlockId",
                table: "ProjectTasks");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTasks_BlockId",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "BlockId",
                table: "ProjectTasks");

            migrationBuilder.AddColumn<string>(
                name: "BlockName",
                table: "ProjectTasks",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
