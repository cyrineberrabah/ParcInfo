using Azure;
using Microsoft.AspNetCore.Authorization;
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
        // GET: Materiel/Create
        [Authorize(Roles = "Admin")] 
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Create(MaterielCreateViewModel vm)
        {
            _logger.LogInformation("POST Create called with TypeMateriel={TypeMateriel}", vm.TypeMateriel);

            // --- Si l'utilisateur a choisi "Autre", récupérer le nouveau type ---
            if (vm.TypeMateriel == "Autre" && !string.IsNullOrWhiteSpace(Request.Form["NouveauType"]))
            {
                vm.TypeMateriel = Request.Form["NouveauType"];
                _logger.LogInformation("Nouveau TypeMateriel saisi par l'utilisateur : {TypeMateriel}", vm.TypeMateriel);
            }

            // --- Validation Marque ---
            if (string.IsNullOrWhiteSpace(vm.Marque))
            {
                ModelState.AddModelError(nameof(vm.Marque), "La marque est obligatoire.");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(vm.Marque, @"^[A-Za-zÀ-ÖØ-öø-ÿ\s]*$"))
            {
                ModelState.AddModelError(nameof(vm.Marque), "La marque ne peut contenir que des lettres.");
            }

            // --- Validation Numéro de série ---
            if (!string.IsNullOrWhiteSpace(vm.NumeroSerie) &&
                !System.Text.RegularExpressions.Regex.IsMatch(vm.NumeroSerie, @"^[0-9]*$"))
            {
                ModelState.AddModelError(nameof(vm.NumeroSerie), "Le numéro de série doit contenir uniquement des chiffres.");
            }
            // Validation conditionnelle côté serveur
            switch (vm.TypeMateriel)
            {
                case "PC":
                    if (string.IsNullOrWhiteSpace(vm.OS))
                        ModelState.AddModelError(nameof(vm.OS), "Le champ OS est obligatoire pour un PC.");

                    if (string.IsNullOrWhiteSpace(vm.RAM))
                        ModelState.AddModelError(nameof(vm.RAM), "La RAM est obligatoire pour un PC.");
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(vm.RAM, @"^\d+\s*Go$"))
                        ModelState.AddModelError(nameof(vm.RAM), "La RAM doit être au format : nombre + 'Go' (ex : 8 Go).");

                    if (string.IsNullOrWhiteSpace(vm.CPU))
                        ModelState.AddModelError(nameof(vm.CPU), "Le CPU est obligatoire pour un PC.");
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(vm.CPU, @"^[A-Za-z0-9\s]+$"))
                        ModelState.AddModelError(nameof(vm.CPU), "Le CPU doit contenir uniquement lettres et chiffres.");

                    if (string.IsNullOrWhiteSpace(vm.Stockage))
                        ModelState.AddModelError(nameof(vm.Stockage), "Le stockage est obligatoire pour un PC.");

                    if (!string.IsNullOrWhiteSpace(vm.AdresseMAC) &&
                        !System.Text.RegularExpressions.Regex.IsMatch(vm.AdresseMAC, @"^([0-9A-Fa-f]{2}:){5}[0-9A-Fa-f]{2}$"))
                        ModelState.AddModelError(nameof(vm.AdresseMAC), "Le format de l'adresse MAC est invalide (ex : 00:1A:2B:3C:4D:5E).");
                    break;

                case "Imprimante":
                    if (string.IsNullOrWhiteSpace(vm.TypeImpression))
                        ModelState.AddModelError(nameof(vm.TypeImpression), "Le type d'impression est obligatoire.");

                    if (string.IsNullOrWhiteSpace(vm.VitesseImpression))
                        ModelState.AddModelError(nameof(vm.VitesseImpression), "La vitesse d'impression est obligatoire.");
                    else if (!new[] { "10 ppm", "20 ppm", "30 ppm", "50 ppm" }.Contains(vm.VitesseImpression))
                        ModelState.AddModelError(nameof(vm.VitesseImpression), "Vitesse d'impression invalide.");

                    if (string.IsNullOrWhiteSpace(vm.Resolution))
                        ModelState.AddModelError(nameof(vm.Resolution), "La résolution est obligatoire.");
                    else if (!new[] { "600 dpi", "1200 dpi", "2400 dpi" }.Contains(vm.Resolution))
                        ModelState.AddModelError(nameof(vm.Resolution), "Résolution invalide.");

                    if (string.IsNullOrWhiteSpace(vm.Connectivite))
                        ModelState.AddModelError(nameof(vm.Connectivite), "La connectivité est obligatoire.");
                    else if (!new[] { "USB", "Wi-Fi", "Ethernet" }.Contains(vm.Connectivite))
                        ModelState.AddModelError(nameof(vm.Connectivite), "Connectivité invalide.");
                    break;

                case "Serveur":
                    // Nombre de slots RAM obligatoire et positif
                    if (!vm.NombreDeSlotsRAM.HasValue)
                        ModelState.AddModelError(nameof(vm.NombreDeSlotsRAM), "Le nombre de slots RAM est requis pour un serveur.");
                    else if (vm.NombreDeSlotsRAM <= 0)
                        ModelState.AddModelError(nameof(vm.NombreDeSlotsRAM), "Le nombre de slots RAM doit être supérieur à 0.");

                    // Type RAID doit être dans la liste autorisée
                    var allowedRAID = new[] { "RAID 0", "RAID 1", "RAID 5", "RAID 10" };
                    if (string.IsNullOrWhiteSpace(vm.TypeRAID))
                        ModelState.AddModelError(nameof(vm.TypeRAID), "Le champ Type RAID est requis pour un serveur.");
                    else if (!allowedRAID.Contains(vm.TypeRAID))
                        ModelState.AddModelError(nameof(vm.TypeRAID), "Le Type RAID choisi n’est pas valide.");

                    // Adresse IP doit avoir un format IPv4 correct
                    if (string.IsNullOrWhiteSpace(vm.AdresseIP))
                        ModelState.AddModelError(nameof(vm.AdresseIP), "Le champ Adresse IP est requis pour un serveur.");
                    else if (!System.Net.IPAddress.TryParse(vm.AdresseIP, out var ip) || ip.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                        ModelState.AddModelError(nameof(vm.AdresseIP), "L’Adresse IP doit être une IPv4 valide.");

                    break;


                case "Scanner":
                    // Vérifie que le type de scanner est renseigné et valide
                    var allowedScannerTypes = new[] { "À plat", "Portable", "Réseau" };
                    if (string.IsNullOrWhiteSpace(vm.TypeScanner))
                        ModelState.AddModelError(nameof(vm.TypeScanner), "Le champ Type Scanner est requis pour un scanner.");
                    else if (!allowedScannerTypes.Contains(vm.TypeScanner))
                        ModelState.AddModelError(nameof(vm.TypeScanner), "Le type de scanner choisi n’est pas valide.");

                    // Vérifie que la vitesse de scan est renseignée et valide
                    var allowedScanSpeeds = new[] { "10 ppm", "20 ppm", "30 ppm" };
                    if (string.IsNullOrWhiteSpace(vm.VitesseScan))
                        ModelState.AddModelError(nameof(vm.VitesseScan), "Le champ Vitesse Scan est requis pour un scanner.");
                    else if (!allowedScanSpeeds.Contains(vm.VitesseScan))
                        ModelState.AddModelError(nameof(vm.VitesseScan), "La vitesse de scan choisie n’est pas valide.");
                    break;


                case "Écran":
                    var allowedSizes = new[] { "21", "24", "27", "32" };
                    if (string.IsNullOrWhiteSpace(vm.Taille))
                        ModelState.AddModelError(nameof(vm.Taille), "Le champ Taille est requis pour un écran.");
                    else if (!allowedSizes.Contains(vm.Taille))
                        ModelState.AddModelError(nameof(vm.Taille), "La taille choisie n’est pas valide.");

                    // Vérification du type d’écran
                    var allowedScreenTypes = new[] { "LED", "IPS", "OLED" };
                    if (string.IsNullOrWhiteSpace(vm.TypeEcran))
                        ModelState.AddModelError(nameof(vm.TypeEcran), "Le champ Type Écran est requis pour un écran.");
                    else if (!allowedScreenTypes.Contains(vm.TypeEcran))
                        ModelState.AddModelError(nameof(vm.TypeEcran), "Le type d’écran choisi n’est pas valide.");

                    // Vérification du temps de réponse
                    var allowedResponseTimes = new[] { "1 ms", "5 ms", "10 ms" };
                    if (string.IsNullOrWhiteSpace(vm.TempsDeReponse))
                        ModelState.AddModelError(nameof(vm.TempsDeReponse), "Le champ Temps de réponse est requis pour un écran.");
                    else if (!allowedResponseTimes.Contains(vm.TempsDeReponse))
                        ModelState.AddModelError(nameof(vm.TempsDeReponse), "Le temps de réponse choisi n’est pas valide.");
                    break;

            }

            // Si le modèle n'est pas valide, renvoyer la vue avec les messages d'erreur
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modèle invalide lors de la création. Erreurs : {Errors}",
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(vm);
            }

            // Création du matériel
            var materiel = new Materiel
            {
                TypeMateriel = vm.TypeMateriel,
                Marque = vm.Marque,
                Modele = vm.Modele,
                NumeroSerie = vm.NumeroSerie,
                Etat = "Disponible",  // <--- ici
                Description = vm.Description, // Peut être null                Etat = "Disponible",
                PreneurId = null,

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
                TempsDeReponse = vm.TempsDeReponse
            };

            _context.Add(materiel);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Matériel ajouté avec succès, Id={Id}", materiel.Id);

            return RedirectToAction(nameof(Index));
        }



        // GET: Materiel/Index
        public IActionResult Index(string search = "", string typeMateriel = "", string etat = "", string sortOrder = "asc", int page = 1, int pageSize = 10)
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

            // Filtrage par état si spécifié
            if (!string.IsNullOrEmpty(etat))
            {
                query = query.Where(m => m.Etat == etat);
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
            ViewBag.Etat = etat; // <-- Nouveau : pour la vue

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
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Materiel materiel)
        {
            if (id != materiel.Id)
                return NotFound();

            // Log initial de l'objet reçu
            _logger.LogInformation("Objet Materiel reçu pour modification : {@Materiel}", materiel);

            // Validation conditionnelle selon le type
            switch (materiel.TypeMateriel)
            {
                case "PC":
                    if (string.IsNullOrWhiteSpace(materiel.OS))
                        ModelState.AddModelError(nameof(materiel.OS), "OS requis pour un PC");
                    if (string.IsNullOrWhiteSpace(materiel.CPU))
                        ModelState.AddModelError(nameof(materiel.CPU), "CPU requis pour un PC");
                    if (string.IsNullOrWhiteSpace(materiel.RAM))
                        ModelState.AddModelError(nameof(materiel.RAM), "RAM requise pour un PC");
                    if (string.IsNullOrWhiteSpace(materiel.Stockage))
                        ModelState.AddModelError(nameof(materiel.Stockage), "Stockage requis pour un PC");
                    break;

                case "Imprimante":
                    if (string.IsNullOrWhiteSpace(materiel.TypeImpression))
                        ModelState.AddModelError(nameof(materiel.TypeImpression), "Type Impression requis pour une imprimante");
                    break;

                case "Serveur":
                    if (!materiel.NombreDeSlotsRAM.HasValue)
                        ModelState.AddModelError(nameof(materiel.NombreDeSlotsRAM), "Nombre de slots RAM requis pour un serveur");
                    if (string.IsNullOrWhiteSpace(materiel.TypeRAID))
                        ModelState.AddModelError(nameof(materiel.TypeRAID), "Type RAID requis pour un serveur");
                    if (string.IsNullOrWhiteSpace(materiel.AdresseIP))
                        ModelState.AddModelError(nameof(materiel.AdresseIP), "Adresse IP requise pour un serveur");
                    break;

                case "Scanner":
                    if (string.IsNullOrWhiteSpace(materiel.TypeScanner))
                        ModelState.AddModelError(nameof(materiel.TypeScanner), "Type Scanner requis");
                    if (string.IsNullOrWhiteSpace(materiel.VitesseScan))
                        ModelState.AddModelError(nameof(materiel.VitesseScan), "Vitesse Scan requise");
                    break;

                case "Écran":
                    if (string.IsNullOrWhiteSpace(materiel.Taille))
                        ModelState.AddModelError(nameof(materiel.Taille), "Taille requise pour un écran");
                    if (string.IsNullOrWhiteSpace(materiel.TypeEcran))
                        ModelState.AddModelError(nameof(materiel.TypeEcran), "Type Écran requis");
                    if (string.IsNullOrWhiteSpace(materiel.TempsDeReponse))
                        ModelState.AddModelError(nameof(materiel.TempsDeReponse), "Temps de réponse requis");
                    break;
            }

            // ⚠ Log toutes les erreurs de ModelState
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    var field = entry.Key;
                    foreach (var error in entry.Value.Errors)
                    {
                        _logger.LogWarning("Erreur ModelState: Champ={Field}, Message={Error}", field, error.ErrorMessage);
                    }
                }
                return View(materiel); // retourne la vue pour corriger
            }

            // Sauvegarde si tout est OK
            try
            {
                _context.Update(materiel);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Matériel modifié avec succès, Id={Id}", materiel.Id);
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


        // GET: Materiel/Delete/5
        [Authorize(Roles = "Admin")] // Seul Admin peut supprimer
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

        [Authorize(Roles = "Admin")] // Seul Admin peut supprimer
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



        // GET: Materiel/AddReference/5
        public async Task<IActionResult> AddReference(int? id)
        {
            if (id == null)
                return NotFound();

            var materiel = await _context.Materiels.FindAsync(id);
            if (materiel == null)
                return NotFound();

            return View(materiel); // On passe le materiel à la vue
        }

        // POST: Materiel/AddReference/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReference(int id, string? Reference)
        {
            var materiel = await _context.Materiels.FindAsync(id);
            if (materiel == null)
                return NotFound();

            materiel.Reference = Reference; // Met à jour la référence
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

   


    }
}


    





