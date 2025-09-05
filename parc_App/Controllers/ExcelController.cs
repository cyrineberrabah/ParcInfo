using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using parc_App.Models;
using System;
using System.IO;
using System.Linq;

namespace parc_App.Controllers
{
    public class ExcelController : Controller
    {
        private readonly Appdatacontext _context;
        private readonly ILogger<ExcelController> _logger;

        public ExcelController(Appdatacontext context, ILogger<ExcelController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult ExportExcel(DateTime? dateDebut, DateTime? dateFin, string typeMateriel, string etat)
        {
            _logger.LogInformation("ExportExcel called with dateDebut={dateDebut}, dateFin={dateFin}, typeMateriel={typeMateriel}, etat={etat}",
                                   dateDebut, dateFin, typeMateriel, etat);

            // Vérification obligatoire : dateDebut si dateFin est renseignée
            if (dateFin.HasValue && !dateDebut.HasValue)
            {
                TempData["ErrorMessage"] = "Vous devez renseigner la date de début si vous choisissez une date de fin !";
                return RedirectToAction("Index", "Materiel"); // redirige vers la page de la table
            }

            // Récupérer tous les matériels avec la dernière affectation
            var query = _context.Materiels
                                .Include(m => m.Preneur)
                                .Select(m => new
                                {
                                    m.Id,
                                    m.TypeMateriel,
                                    m.Marque,
                                    m.Modele,
                                    m.Etat,
                                    Preneur = m.Preneur != null ? m.Preneur.Nom + " " + m.Preneur.Prenom : "Non attribué",
                                    DerniereAffectation = _context.HistoriqueAffectations
                                        .Where(h => h.MaterielId == m.Id)
                                        .OrderByDescending(h => h.DateAffectation)
                                        .Select(h => h.DateAffectation)
                                        .FirstOrDefault()
                                })
                                .AsQueryable();

            // Appliquer les filtres
            if (dateDebut.HasValue)
            {
                _logger.LogInformation("Filtrage sur dateDebut: {dateDebut}", dateDebut.Value);
                query = query.Where(m => m.DerniereAffectation >= dateDebut.Value);
            }

            if (dateFin.HasValue)
            {
                _logger.LogInformation("Filtrage sur dateFin: {dateFin}", dateFin.Value);
                query = query.Where(m => m.DerniereAffectation <= dateFin.Value);
            }

            if (!string.IsNullOrEmpty(typeMateriel))
            {
                _logger.LogInformation("Filtrage sur typeMateriel: {typeMateriel}", typeMateriel);
                query = query.Where(m => m.TypeMateriel == typeMateriel);
            }

            if (!string.IsNullOrEmpty(etat))
            {
                _logger.LogInformation("Filtrage sur etat: {etat}", etat);
                query = query.Where(m => m.Etat == etat);
            }

            var data = query.ToList();

            _logger.LogInformation("Nombre de matériels après filtrage: {count}", data.Count);

            // Création du fichier Excel (comme dans ton code)
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Materiels");

                // Entêtes
                var headers = new[] { "Id", "Type", "Marque", "Modèle", "État", "Preneur", "Dernière affectation" };
                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cell(1, i + 1).Value = headers[i];
                    ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell(1, i + 1).Style.Font.Bold = true;
                    ws.Cell(1, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // Remplir les données
                for (int i = 0; i < data.Count; i++)
                {
                    ws.Cell(i + 2, 1).Value = data[i].Id;
                    ws.Cell(i + 2, 2).Value = data[i].TypeMateriel;
                    ws.Cell(i + 2, 3).Value = data[i].Marque;
                    ws.Cell(i + 2, 4).Value = data[i].Modele;
                    ws.Cell(i + 2, 5).Value = data[i].Etat;
                    ws.Cell(i + 2, 6).Value = data[i].Preneur;
                    ws.Cell(i + 2, 7).Value = data[i].DerniereAffectation != default(DateTime)
                        ? data[i].DerniereAffectation.ToString("dd/MM/yyyy")
                        : "Non affecté";

                    var etatCell = ws.Cell(i + 2, 5);
                    switch (data[i].Etat)
                    {
                        case "Disponible":
                            etatCell.Style.Fill.BackgroundColor = XLColor.LightGreen;
                            break;
                        case "Occupé":
                            etatCell.Style.Fill.BackgroundColor = XLColor.LightSalmon;
                            break;
                        default:
                            etatCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                            break;
                    }
                }

                ws.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();
                    return File(stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "Materiels.xlsx");
                }
            }
        }

    }
}
