using Microsoft.EntityFrameworkCore.Migrations;

namespace EmpMgr.Migrations
{
    public partial class EnabledCascadeDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyScrums_AspNetUsers_EmployeeId",
                table: "DailyScrums");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyScrums_AspNetUsers_EmployeeId",
                table: "DailyScrums",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyScrums_AspNetUsers_EmployeeId",
                table: "DailyScrums");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyScrums_AspNetUsers_EmployeeId",
                table: "DailyScrums",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
