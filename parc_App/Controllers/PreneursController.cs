using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using parc_App.Models;
using parc_App.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace parc_App.Controllers
{
    public class PreneursController : Controller
    {
        private readonly Appdatacontext _context;
        private readonly ILogger<PreneursController> _logger;

        // ✅ Injecter le logger dans le constructeur
        public PreneursController(Appdatacontext context, ILogger<PreneursController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string search = "", string sortOrder = "asc", string departement = "", int page = 1, int pageSize = 10)
        {
            var query = _context.Preneurs.Include(p => p.Departement).Where(p => !p.IsDeleted);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    p.Nom.Contains(search) ||
                    p.Prenom.Contains(search) ||
                    p.Email.Contains(search) ||
                    (p.Departement != null && p.Departement.Nom.Contains(search))
                );
            }

            if (!string.IsNullOrEmpty(departement))
                query = query.Where(p => p.Departement.Nom == departement);

            query = sortOrder.ToLower() == "desc"
                ? query.OrderByDescending(p => p.Nom)
                : query.OrderBy(p => p.Nom);

            var totalPreneurs = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalPreneurs / (double)pageSize);

            var preneurs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPreneurs = totalPreneurs;
            ViewBag.Search = search;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SelectedDepartement = departement;
            ViewBag.Departements = await _context.Departements.ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_PreneursTablePartial", preneurs); // ⚡️ PartialView à créer avec juste le <tbody>

            return View(preneurs);
        }

        // GET: /Preneurs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var preneur = await _context.Preneurs
                .Include(p => p.Departement)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (preneur == null) return NotFound();

            return View(preneur);
        }

        // GET: /Preneurs/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Departements = await _context.Departements.ToListAsync();
            return View();
        }

        // POST: /Preneurs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(PreneurCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var preneur = new Preneur
                {
                    Nom = vm.Nom,
                    Prenom = vm.Prenom,
                    Email = vm.Email,
                    Telephone = vm.Telephone,
                    DepartementId = vm.DepartementId
                };

                _context.Add(preneur);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departements = await _context.Departements.ToListAsync();
            return View(vm);
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)


        {
            if (id == null) return NotFound();

            var preneur = await _context.Preneurs.FindAsync(id);
            if (preneur == null) return NotFound();

            ViewBag.Departements = await _context.Departements.ToListAsync();

            // Si c’est une requête AJAX, on peut retourner un PartialView ou JSON
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(preneur);
            }

            return View(preneur);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Preneur preneur)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Modèle invalide", errors = ModelState });

            if (preneur.DepartementId <= 0 ||
                !await _context.Departements.AnyAsync(d => d.Id == preneur.DepartementId))
                return BadRequest(new { message = "Le département est obligatoire ou invalide." });

            _context.Preneurs.Update(preneur);
            await _context.SaveChangesAsync();

            var dep = await _context.Departements
                                    .Where(d => d.Id == preneur.DepartementId)
                                    .Select(d => d.Nom)
                                    .FirstOrDefaultAsync();

            return Json(new
            {
                success = true,
                preneur = new
                {
                    id = preneur.Id,
                    nom = preneur.Nom,
                    prenom = preneur.Prenom,
                    email = preneur.Email,
                    telephone = preneur.Telephone,
                    departementId = preneur.DepartementId,
                    departementNom = dep
                }
            });
        }




        // GET: /Preneurs/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var preneur = await _context.Preneurs
                .Include(p => p.Departement)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (preneur == null) return NotFound();

            return View(preneur);
        }

        // POST: /Preneurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var preneur = await _context.Preneurs.FindAsync(id);
            if (preneur != null)
            {
                preneur.IsDeleted = true; // Soft delete
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Historique
        public IActionResult HistoriquePreneur(int id)
        {
            var historique = _context.HistoriqueAffectations
                .Include(h => h.Materiel)
                .Where(h => h.PreneurId == id)
                .OrderByDescending(h => h.DateAffectation)
                .ToList();

            var preneur = _context.Preneurs.Find(id);
            ViewBag.PreneurNom = preneur.Nom + " " + preneur.Prenom;

            return View(historique);
        }

        // ✅ Méthode privée pour vérifier existence
        private async Task<bool> PreneurExists(int id)
        {
            return await _context.Preneurs.AnyAsync(e => e.Id == id);
        }
    }
}
