using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace parc_App.Migrations
{
    /// <inheritdoc />
    public partial class AddMaterielSpecificFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdresseIP",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdresseMAC",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationsInstallees",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CPU",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Connectivite",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Couleur",
                table: "Materiels",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NombreDeSlotsRAM",
                table: "Materiels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OS",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RAM",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Resolution",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Stockage",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Taille",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TempsDeReponse",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypeEcran",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypeImpression",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypeRAID",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypeScanner",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VitesseImpression",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VitesseScan",
                table: "Materiels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdresseIP",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "AdresseMAC",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "ApplicationsInstallees",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "CPU",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "Connectivite",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "Couleur",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "NombreDeSlotsRAM",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "OS",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "RAM",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "Resolution",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "Stockage",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "Taille",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "TempsDeReponse",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "TypeEcran",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "TypeImpression",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "TypeRAID",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "TypeScanner",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "VitesseImpression",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "VitesseScan",
                table: "Materiels");
        }
    }
}
