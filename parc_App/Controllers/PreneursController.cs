using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using parc_App.Models;
using System.Linq;
using System.Threading.Tasks;

namespace parc_App.Controllers
{
    public class PreneursController : Controller
    {
        private readonly Appdatacontext _context;

        public PreneursController(Appdatacontext context)
        {
            _context = context;
        }

        // GET: /Preneurs
        public async Task<IActionResult> Index(string search = "", string sortOrder = "asc", string departement = "", int page = 1, int pageSize = 10)
        {
            // Base query : uniquement les preneurs non supprimés
            var query = _context.Preneurs
                .Where(p => !p.IsDeleted);

            // Filtrage par recherche (nom ou prénom)
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Nom.Contains(search) || p.Prenom.Contains(search));
            }

            // Filtrage par département si renseigné
            if (!string.IsNullOrEmpty(departement))
            {
                query = query.Where(p => p.Departement == departement);
            }

            // Tri alphabétique
            query = sortOrder.ToLower() == "desc"
                ? query.OrderByDescending(p => p.Nom)
                : query.OrderBy(p => p.Nom);

            // Pagination
            var totalPreneurs = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalPreneurs / (double)pageSize);

            var preneurs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Info pagination & filtres pour la vue
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPreneurs = totalPreneurs;
            ViewBag.Search = search;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SelectedDepartement = departement;

            // Liste des départements pour le dropdown
            ViewBag.Departements = await _context.Preneurs
                .Where(p => !p.IsDeleted)
                .Select(p => p.Departement)
                .Distinct()
                .ToListAsync();

            return View(preneurs);
        }


        // GET: /Preneurs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var preneur = await _context.Preneurs
                .FirstOrDefaultAsync(p => p.Id == id);

            if (preneur == null) return NotFound();

            return View(preneur);
        }

        // GET: /Preneurs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Preneurs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Preneur preneur)
        {
           

            if (ModelState.IsValid)
            {
                _context.Preneurs.Add(preneur);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
           }

            return View(preneur);
        }

        // GET: /Preneurs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var preneur = await _context.Preneurs.FindAsync(id);
            if (preneur == null) return NotFound();

            return View(preneur);
        }

        // POST: /Preneurs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Preneur preneur)
        {
            if (id != preneur.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Preneurs.Update(preneur);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await PreneurExists(preneur.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(preneur);
        }

        // GET: /Preneurs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var preneur = await _context.Preneurs
                .FirstOrDefaultAsync(p => p.Id == id);

            if (preneur == null) return NotFound();

            return View(preneur);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var preneur = await _context.Preneurs.FindAsync(id);
            if (preneur != null)
            {
                // Soft delete
                preneur.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        private async Task<bool> PreneurExists(int id)
        {
            return await _context.Preneurs.AnyAsync(e => e.Id == id);
        }




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

    }
}
