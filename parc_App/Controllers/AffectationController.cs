using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using parc_App.Models;
using parc_App.ViewModel;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using parc_App.Models;

namespace parc_App.Controllers
{
    public class AffectationController : Controller
    {
        private readonly Appdatacontext _context;
        private readonly ILogger<AffectationController> _logger;

        public AffectationController(Appdatacontext context, ILogger<AffectationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: liste des matériels avec pagination
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var totalMateriels = await _context.Materiels.CountAsync(); // inclut les matériels supprimés

            var totalPages = (int)Math.Ceiling(totalMateriels / (double)pageSize);

            var model = await _context.Materiels
       .Include(m => m.Preneur)
       .OrderBy(m => m.Id)
       .Skip((page - 1) * pageSize)
       .Take(pageSize)
       .Select(m => new MaterielAffectationViewModel
       {
           MaterielId = m.Id,
           TypeMateriel = m.TypeMateriel,
           Marque = m.Marque,
           Modele = m.Modele,
           Etat = m.IsDeleted ? "Amorti" : m.Etat,  // <-- état amorti si supprimé
           PreneurId = m.PreneurId,
           PreneurNomComplet = m.Preneur != null ? m.Preneur.Nom + " " + m.Preneur.Prenom : null
       })
       .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalMateriels = totalMateriels;

            return View(model);
        }




        // GET : formulaire Affecter
        // GET : formulaire Affecter
        [HttpGet]
        public async Task<IActionResult> Affecter(int id)
        {
            var materiel = await _context.Materiels.FindAsync(id);
            if (materiel == null) return NotFound();

            // Récupération uniquement des preneurs non supprimés
            var preneurs = await _context.Preneurs
                .Where(p => !p.IsDeleted) // <-- exclut les preneurs supprimés
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nom + " " + p.Prenom
                })
                .ToListAsync();

            var vm = new MaterielAffectationViewModel
            {
                MaterielId = materiel.Id,
                PreneurId = materiel.PreneurId,
                PreneursDisponibles = preneurs
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Affecter(int id, int? preneurId) // <- int? ici
        {
            var materiel = await _context.Materiels.FindAsync(id);
            if (materiel == null) return NotFound();

            // On met à jour l'état et le preneur
            materiel.PreneurId = preneurId;
            materiel.Etat = preneurId.HasValue ? "Occupé" : "Disponible";

            // ➕ Historique
            var historique = new HistoriqueAffectation
            {
                MaterielId = id,
                PreneurId = preneurId, // sera null si restitution
                DateAffectation = DateTime.Now,
                Action = preneurId.HasValue ? "Affectation" : "Restitution"
            };

            _context.HistoriqueAffectations.Add(historique);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Historique(int id)
        {
            var historique = await _context.HistoriqueAffectations
                .Include(h => h.Materiel)
                .Include(h => h.Preneur)
                .Where(h => h.MaterielId == id)
                .ToListAsync();

            return View(historique);
        }



    }
}
