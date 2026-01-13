using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WasmProject.Migrations
{
    public partial class AddEmailToUserDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // هذا السطر هو الوحيد الذي نحتاجه لتحديث قاعدة بيانات وسم
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserDB",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserDB");
        }
    }
}