using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace parc_App.Migrations
{
    /// <inheritdoc />
    public partial class HistoriquePreneurNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoriqueAffectations_Preneurs_PreneurId",
                table: "HistoriqueAffectations");

            migrationBuilder.AlterColumn<int>(
                name: "PreneurId",
                table: "HistoriqueAffectations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriqueAffectations_Preneurs_PreneurId",
                table: "HistoriqueAffectations",
                column: "PreneurId",
                principalTable: "Preneurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoriqueAffectations_Preneurs_PreneurId",
                table: "HistoriqueAffectations");

            migrationBuilder.AlterColumn<int>(
                name: "PreneurId",
                table: "HistoriqueAffectations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriqueAffectations_Preneurs_PreneurId",
                table: "HistoriqueAffectations",
                column: "PreneurId",
                principalTable: "Preneurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
