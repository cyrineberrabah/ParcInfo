using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace parc_App.Migrations
{
    /// <inheritdoc />
    public partial class Adddepid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Departement",
                table: "Preneurs");

            migrationBuilder.AddColumn<int>(
                name: "DepartementId",
                table: "Preneurs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Preneurs_DepartementId",
                table: "Preneurs",
                column: "DepartementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Preneurs_Departements_DepartementId",
                table: "Preneurs",
                column: "DepartementId",
                principalTable: "Departements",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Preneurs_Departements_DepartementId",
                table: "Preneurs");

            migrationBuilder.DropIndex(
                name: "IX_Preneurs_DepartementId",
                table: "Preneurs");

            migrationBuilder.DropColumn(
                name: "DepartementId",
                table: "Preneurs");

            migrationBuilder.AddColumn<string>(
                name: "Departement",
                table: "Preneurs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
