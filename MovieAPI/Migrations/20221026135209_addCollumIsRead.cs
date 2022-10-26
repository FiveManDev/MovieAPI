using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAPI.Migrations
{
    public partial class addCollumIsRead : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Ticket",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "644405e3-412f-4ccd-8de9-cf2912000006",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "632729d0-9f8e-4209-b2e2-d77aaa912b5b");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Ticket");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "632729d0-9f8e-4209-b2e2-d77aaa912b5b",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "644405e3-412f-4ccd-8de9-cf2912000006");
        }
    }
}
