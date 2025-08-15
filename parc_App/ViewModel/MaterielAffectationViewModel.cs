using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace parc_App.ViewModel
{
    public class MaterielAffectationViewModel
    {
        public int MaterielId { get; set; }
        public string TypeMateriel { get; set; }
        public string Marque { get; set; }
        public string Modele { get; set; }
        public string Etat { get; set; }

        public int? PreneurId { get; set; }

        // Liste pour le formulaire
        public List<SelectListItem> PreneursDisponibles { get; set; } = new List<SelectListItem>();

        // Pour afficher le nom complet dans la vue Index
        public string PreneurNomComplet { get; set; }



    }
}
