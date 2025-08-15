using parc_App.Models;
namespace parc_App.Models
{
    public class HistoriqueAffectation
    {
        public int Id { get; set; }
        public int MaterielId { get; set; }
        public Materiel Materiel { get; set; }

        public int? PreneurId { get; set; }

        public Preneur Preneur { get; set; }

        public DateTime DateAffectation { get; set; }
        public string Action { get; set; } // "Affecté", "Restitué", etc.



        public DateTime? DateRetour { get; set; }
    }

}