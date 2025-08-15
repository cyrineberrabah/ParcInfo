using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace parc_App.Migrations
{
    /// <inheritdoc />
    public partial class AddPreneurIdToMateriel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PreneurId",
                table: "Materiels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materiels_PreneurId",
                table: "Materiels",
                column: "PreneurId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materiels_Preneurs_PreneurId",
                table: "Materiels",
                column: "PreneurId",
                principalTable: "Preneurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materiels_Preneurs_PreneurId",
                table: "Materiels");

            migrationBuilder.DropIndex(
                name: "IX_Materiels_PreneurId",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "PreneurId",
                table: "Materiels");
        }
    }
}
