using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using parc_App.Models;
using System.Globalization;
using System.Linq;

[Authorize(AuthenticationSchemes = "MyCookieAuth")]
public class AdminController : Controller
{
    private readonly Appdatacontext _context;

    public AdminController(Appdatacontext context)
    {
        _context = context;
    }

    public IActionResult Dashboard()
    {
        // Charger tous les matériels
        var materiels = _context.Materiels.ToList();

        // Card Matériel (seulement disponibles)
        ViewBag.MaterielsCount = materiels.Count(m => m.Etat == "Disponible" && !m.IsDeleted);

        // Card Preneurs
        ViewBag.PreneursCount = _context.Preneurs.Count(p => !p.IsDeleted);

        // Card PC disponibles
        ViewBag.PCCount = materiels.Count(m => m.TypeMateriel == "PC" && m.Etat == "Disponible");

        // Card Imprimantes disponibles
        ViewBag.ImprimantesCount = materiels.Count(m => m.TypeMateriel == "Imprimante" && m.Etat == "Disponible");
        // ---------------- PIE CHART ----------------
        var etatCounts = materiels
            .Select(m => m.IsDeleted ? "Amorti" : m.Etat)
            .GroupBy(e => e)
            .Select(g => new { Etat = g.Key, Count = g.Count() })
            .ToList();

        ViewBag.PieEtats = etatCounts.Select(x => x.Etat).ToArray();
        ViewBag.PieCounts = etatCounts.Select(x => x.Count).ToArray();

        // ---------------- LINE CHART ----------------
        var affectations = _context.HistoriqueAffectations
            .Where(h => h.DateAffectation != null)
            .GroupBy(h => h.DateAffectation.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToList();

        ViewBag.AffectationDates = affectations.Select(a => a.Date.ToString("yyyy-MM-dd")).ToArray();
        ViewBag.AffectationCounts = affectations.Select(a => a.Count).ToArray();

        // ---------------- PROGRESS BARS (par type) ----------------
        var typeCounts = materiels
            .GroupBy(m => m.TypeMateriel)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToList();

        ViewBag.TypeCounts = typeCounts;
        ViewBag.TotalMateriels = materiels.Count;


        // ---------------- HISTOGRAMME Types vs États ----------------
        // Préparer une liste spécifique pour l'histogramme
        var materielsHisto = _context.Materiels
            .Select(m => new
            {
                Type = m.TypeMateriel,
                Etat = m.IsDeleted ? "Amorti" : m.Etat
            })
            .ToList();

        // Types et États distincts
        var typeLabels = materielsHisto.Select(m => m.Type).Distinct().ToList();
        var etatLabels = materielsHisto.Select(m => m.Etat).Distinct().ToList();

        // Construire la matrice [etat][type]
        var typeEtatCounts = etatLabels
            .Select(etat => typeLabels
                .Select(type => materielsHisto.Count(m => m.Type == type && m.Etat == etat))
                .ToList()
            ).ToList();

        ViewBag.TypeLabels = typeLabels;
        ViewBag.EtatLabels = etatLabels;
        ViewBag.TypeEtatCounts = typeEtatCounts;







        return View();
    }




    public IActionResult GetMaterielCount(string type)
    {
        var query = _context.Materiels
                            .Where(m => !m.IsDeleted && m.Etat == "Disponible");

        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(m => m.TypeMateriel == type);
        }

        return Json(query.Count());
    }

}
