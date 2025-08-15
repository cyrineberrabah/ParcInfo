using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace parc_App.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToMateriel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoriqueAffectations_Preneurs_PreneurId",
                table: "HistoriqueAffectations");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Materiels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriqueAffectations_Preneurs_PreneurId",
                table: "HistoriqueAffectations",
                column: "PreneurId",
                principalTable: "Preneurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoriqueAffectations_Preneurs_PreneurId",
                table: "HistoriqueAffectations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Materiels");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriqueAffectations_Preneurs_PreneurId",
                table: "HistoriqueAffectations",
                column: "PreneurId",
                principalTable: "Preneurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
