using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace User_Management.Migrations
{
    public partial class AddDepartementInUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "departement",
                table: "tmp_role_user_ref",
                type: "character varying",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "departement",
                table: "md_role_user_ref",
                type: "character varying",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "departement",
                table: "lg_role_user_ref",
                type: "character varying",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "departement",
                table: "tmp_role_user_ref");

            migrationBuilder.DropColumn(
                name: "departement",
                table: "md_role_user_ref");

            migrationBuilder.DropColumn(
                name: "departement",
                table: "lg_role_user_ref");
        }
    }
}
