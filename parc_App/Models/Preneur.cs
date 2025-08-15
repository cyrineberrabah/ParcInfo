namespace parc_App.Models
{
    public class Preneur
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Departement { get; set; }
        public bool IsDeleted { get; set; } = false;


        // Navigation vers les matériels (optionnel si relation inverse définie)


        // Liste des matériels affectés
        public ICollection<Materiel>? Materiels { get; set; }
    }
}

