using System.ComponentModel.DataAnnotations;

namespace parc_App.ViewModels
{
    public class PreneurCreateViewModel
    {
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿ\s\-]+$", ErrorMessage = "Le nom ne doit contenir que des lettres.")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿ\s\-]+$", ErrorMessage = "Le prénom ne doit contenir que des lettres.")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Email invalide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le téléphone est obligatoire.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Le téléphone doit contenir exactement 8 chiffres.")]
        public string Telephone { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner un département.")]
        public int DepartementId { get; set; }
    }
}
