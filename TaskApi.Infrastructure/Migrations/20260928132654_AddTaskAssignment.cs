using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedUserId",
                table: "TaskItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_AssignedUserId",
                table: "TaskItems",
                column: "AssignedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_Users_AssignedUserId",
                table: "TaskItems",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_Users_AssignedUserId",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_AssignedUserId",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "AssignedUserId",
                table: "TaskItems");
        }
    }
}
