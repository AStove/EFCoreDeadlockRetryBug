using Microsoft.EntityFrameworkCore.Migrations;

namespace EFCoreDeadlockRetryBug.Migrations
{
    public partial class Haircolor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HairColor",
                table: "Parents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HairColor",
                table: "Parents");
        }
    }
}
