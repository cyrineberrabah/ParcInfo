using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace parc_App.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartementRelationToPreneur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Preneurs_Departements_DepartementId",
                table: "Preneurs");

            migrationBuilder.AddForeignKey(
                name: "FK_Preneurs_Departements_DepartementId",
                table: "Preneurs",
                column: "DepartementId",
                principalTable: "Departements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Preneurs_Departements_DepartementId",
                table: "Preneurs");

            migrationBuilder.AddForeignKey(
                name: "FK_Preneurs_Departements_DepartementId",
                table: "Preneurs",
                column: "DepartementId",
                principalTable: "Departements",
                principalColumn: "Id");
        }
    }
}
