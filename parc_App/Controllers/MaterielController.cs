using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using parc_App.Models;
using parc_App.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace parc_App.Controllers
{
    public class MaterielController : Controller
    {
        private readonly Appdatacontext _context;
        private readonly ILogger<MaterielController> _logger;

        public MaterielController(Appdatacontext context, ILogger<MaterielController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Materiel/Create
        public IActionResult Create()
        {
            _logger.LogInformation("GET Create called");
            return View(new MaterielCreateViewModel());
        }
        private bool MaterielExists(int id)
        {
            return _context.Materiels.Any(e => e.Id == id);
        }
        // POST: Materiel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaterielCreateViewModel vm)
        {
            _logger.LogInformation("POST Create called with TypeMateriel={TypeMateriel}", vm.TypeMateriel);

            // Validation conditionnelle selon le type
            if (vm.TypeMateriel == "PC")
            {
                if (string.IsNullOrWhiteSpace(vm.OS))
                    ModelState.AddModelError("OS", "Le champ OS est requis pour un PC.");
                if (string.IsNullOrWhiteSpace(vm.CPU))
                    ModelState.AddModelError("CPU", "Le champ CPU est requis pour un PC.");
                if (string.IsNullOrWhiteSpace(vm.RAM))
                    ModelState.AddModelError("RAM", "Le champ RAM est requis pour un PC.");
                if (string.IsNullOrWhiteSpace(vm.Stockage))
                    ModelState.AddModelError("Stockage", "Le champ Stockage est requis pour un PC.");
            }
            else if (vm.TypeMateriel == "Imprimante")
            {
                if (string.IsNullOrWhiteSpace(vm.TypeImpression))
                    ModelState.AddModelError("TypeImpression", "Le champ Type Impression est requis pour une imprimante.");
            }
            else if (vm.TypeMateriel == "Serveur")
            {
                if (vm.NombreDeSlotsRAM == null)
                    ModelState.AddModelError("NombreDeSlotsRAM", "Le nombre de slots RAM est requis pour un serveur.");
                if (string.IsNullOrWhiteSpace(vm.TypeRAID))
                    ModelState.AddModelError("TypeRAID", "Le champ Type RAID est requis pour un serveur.");
                if (string.IsNullOrWhiteSpace(vm.AdresseIP))
                    ModelState.AddModelError("AdresseIP", "Le champ Adresse IP est requis pour un serveur.");
            }
            else if (vm.TypeMateriel == "Scanner")
            {
                if (string.IsNullOrWhiteSpace(vm.TypeScanner))
                    ModelState.AddModelError("TypeScanner", "Le champ Type Scanner est requis pour un scanner.");
                if (string.IsNullOrWhiteSpace(vm.VitesseScan))
                    ModelState.AddModelError("VitesseScan", "Le champ Vitesse Scan est requis pour un scanner.");
            }
            else if (vm.TypeMateriel == "Écran")
            {
                if (string.IsNullOrWhiteSpace(vm.Taille))
                    ModelState.AddModelError("Taille", "Le champ Taille est requis pour un écran.");
                if (string.IsNullOrWhiteSpace(vm.TypeEcran))
                    ModelState.AddModelError("TypeEcran", "Le champ Type Écran est requis pour un écran.");
                if (string.IsNullOrWhiteSpace(vm.TempsDeReponse))
                    ModelState.AddModelError("TempsDeReponse", "Le champ Temps de réponse est requis pour un écran.");
            }

            if (ModelState.IsValid)
            {
                var materiel = new Materiel
                {
                    TypeMateriel = vm.TypeMateriel,
                    Marque = vm.Marque,
                    Modele = vm.Modele,
                    NumeroSerie = vm.NumeroSerie,
                    Description = vm.Description,

                    OS = vm.OS,
                    RAM = vm.RAM,
                    CPU = vm.CPU,
                    Stockage = vm.Stockage,
                    ApplicationsInstallees = vm.ApplicationsInstallees,
                    AdresseMAC = vm.AdresseMAC,

                    TypeImpression = vm.TypeImpression,
                    VitesseImpression = vm.VitesseImpression,
                    Resolution = vm.Resolution,
                    Connectivite = vm.Connectivite,
                    Couleur = vm.Couleur,

                    NombreDeSlotsRAM = vm.NombreDeSlotsRAM,
                    TypeRAID = vm.TypeRAID,
                    AdresseIP = vm.AdresseIP,

                    TypeScanner = vm.TypeScanner,
                    VitesseScan = vm.VitesseScan,

                    Taille = vm.Taille,
                    TypeEcran = vm.TypeEcran,
                    TempsDeReponse = vm.TempsDeReponse,

                    // ✅ Initialisation automatique de l'état et preneur
                    Etat = "Disponible",
                    PreneurId = null
                };

                _context.Add(materiel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Matériel ajouté avec succès, Id={Id}", materiel.Id);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                _logger.LogWarning("Modèle invalide lors de la création. Erreurs : {Errors}",
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(vm);
            }
        }

        // GET: Materiel/Index
        public IActionResult Index(string search = "", string typeMateriel = "", string sortOrder = "asc", int page = 1, int pageSize = 10)
        {
            // Base query sur les matériels non supprimés
            var query = _context.Materiels.Where(m => !m.IsDeleted);

            // Recherche sur TypeMateriel, Marque, Modele ou NumeroSerie
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m =>
                    m.TypeMateriel.Contains(search) ||
                    m.Marque.Contains(search) ||
                    m.Modele.Contains(search) ||
                    m.NumeroSerie.Contains(search)
                );
            }

            // Filtrage par type de matériel
            if (!string.IsNullOrEmpty(typeMateriel))
            {
                query = query.Where(m => m.TypeMateriel == typeMateriel);
            }

            // Tri par Marque
            query = sortOrder == "desc" ? query.OrderByDescending(m => m.Marque) : query.OrderBy(m => m.Marque);

            // Pagination
            var totalMateriels = query.Count();
            var totalPages = (int)Math.Ceiling(totalMateriels / (double)pageSize);
            var materiels = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Passer infos à la vue
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalMateriels = totalMateriels;
            ViewBag.Search = search;
            ViewBag.SelectedTypeMateriel = typeMateriel;
            ViewBag.SortOrder = sortOrder;

            // Dropdown des types matériels
            ViewBag.TypeMateriels = _context.Materiels
                .Where(m => !m.IsDeleted)
                .Select(m => m.TypeMateriel)
                .Distinct()
                .ToList();

            return View(materiels);
        }



        // GET: Materiel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var materiel = await _context.Materiels.FirstOrDefaultAsync(m => m.Id == id);
            if (materiel == null)
                return NotFound();

            return View(materiel);
        }

        // GET: Materiel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var materiel = await _context.Materiels.FindAsync(id);
            if (materiel == null)
                return NotFound();

            // Optionnel : Mapper Materiel vers MaterielCreateViewModel si tu utilises un VM dans la vue Edit
            // Ici, on retourne directement le modèle Materiel pour simplifier
            return View(materiel);
        }

        // POST: Materiel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Materiel materiel)
        {
            if (id != materiel.Id)
                return NotFound();

           // if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(materiel);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Materiel modifié avec succès, Id={Id}", materiel.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterielExists(materiel.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(materiel);
        }

        // GET: Materiel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var materiel = await _context.Materiels
                .FirstOrDefaultAsync(m => m.Id == id);

            if (materiel == null)
                return NotFound();

            return View(materiel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var materiel = await _context.Materiels.FindAsync(id);
            if (materiel != null)
            {
                materiel.IsDeleted = true;
                materiel.Etat = "Amorti"; // optionnel
                await _context.SaveChangesAsync();
                _logger.LogInformation("Materiel marqué comme supprimé, Id={Id}", id);
            }
            return RedirectToAction(nameof(Index));
        }




    }
}


    





