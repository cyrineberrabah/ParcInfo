using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using parc_App.Models;
using Microsoft.AspNetCore.Authorization;
namespace parc_App.Controllers
{
    public class DepartementsController : Controller
    {
        private readonly Appdatacontext _context;

        public DepartementsController(Appdatacontext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int PageSize = 5;

            int totalItems = await _context.Departements.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / PageSize);

            page = Math.Max(1, Math.Min(page, totalPages));

            var departements = await _context.Departements
                .OrderBy(d => d.Id)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(departements);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal(Departement departement)
        {
            if (string.IsNullOrWhiteSpace(departement.Nom))
                return Json(new { success = false, error = "Le nom est requis." });

            // Validation côté serveur : première lettre majuscule + lettres seulement
            if (!System.Text.RegularExpressions.Regex.IsMatch(departement.Nom, @"^[A-Z][a-zA-Z]*$"))
            {
                return Json(new { success = false, error = "Le nom doit commencer par une majuscule et ne contenir que des lettres." });
            }

            // Vérifier si le département existe déjà
            bool exists = await _context.Departements.AnyAsync(d => d.Nom == departement.Nom);
            if (exists)
            {
                return Json(new { success = false, error = "Un département avec ce nom existe déjà." });
            }

            _context.Departements.Add(departement);
            await _context.SaveChangesAsync();

            return Json(new { success = true, id = departement.Id, nom = departement.Nom });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(Departement departement)
        {
            if (string.IsNullOrWhiteSpace(departement.Nom))
                return Json(new { success = false, error = "Le nom est requis." });

            // Validation côté serveur : première lettre majuscule + lettres seulement
            if (!System.Text.RegularExpressions.Regex.IsMatch(departement.Nom, @"^[A-Z][a-zA-Z]*$"))
            {
                return Json(new { success = false, error = "Le nom doit commencer par une majuscule et ne contenir que des lettres." });
            }

            // Vérifier si un autre département a déjà ce nom
            bool exists = await _context.Departements.AnyAsync(d => d.Nom == departement.Nom && d.Id != departement.Id);
            if (exists)
            {
                return Json(new { success = false, error = "Un département avec ce nom existe déjà." });
            }

            _context.Departements.Update(departement);
            await _context.SaveChangesAsync();

            return Json(new { success = true, id = departement.Id, nom = departement.Nom });
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteModal(int id)
        {
            var departement = await _context.Departements.FindAsync(id);
            if (departement == null)
                return Json(new { success = false, error = "Département introuvable." });

            _context.Departements.Remove(departement);
            await _context.SaveChangesAsync();

            return Json(new { success = true, id });
        }
    }
}
