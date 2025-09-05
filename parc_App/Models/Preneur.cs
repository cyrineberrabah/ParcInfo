using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace parc_App.Models
{
    public class Preneur
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        // Navigation vers les matériels (optionnel si relation inverse définie)
        public int? DepartementId { get; set; }
        public Departement? Departement { get; set; }

        public string Telephone { get; set; }
        public bool IsDeleted { get; set; } = false;




        // Liste des matériels affectés
        public ICollection<Materiel>? Materiels { get; set; }
    }
}

