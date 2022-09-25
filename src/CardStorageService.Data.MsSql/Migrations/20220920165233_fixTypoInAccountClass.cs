using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardStorageService.Data.MsSql.Migrations
{
    public partial class fixTypoInAccountClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Locker",
                table: "Accounts",
                newName: "Locked");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Locked",
                table: "Accounts",
                newName: "Locker");
        }
    }
}
