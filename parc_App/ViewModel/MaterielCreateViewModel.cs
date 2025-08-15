namespace parc_App.ViewModel
{
    public class MaterielCreateViewModel
    {
        public string TypeMateriel { get; set; }
        public string Marque { get; set; }
        public string Modele { get; set; }
        public string NumeroSerie { get; set; }
        public string Etat { get; set; }
        public string Description { get; set; }

        public string? OS { get; set; }
        public string? RAM { get; set; }
        public string? CPU { get; set; }
        public string? Stockage { get; set; }
        public string? ApplicationsInstallees { get; set; }
        public string? AdresseMAC { get; set; }

        // Imprimante
        public string? TypeImpression { get; set; }
        public string? VitesseImpression { get; set; }
        public string? Resolution { get; set; }
        public string? Connectivite { get; set; }
        public bool Couleur { get; set; }  // Peut rester bool (non nullable)

        // Serveur
        public int? NombreDeSlotsRAM { get; set; }
        public string? TypeRAID { get; set; }
        public string? AdresseIP { get; set; }

        // Scanner
        public string? TypeScanner { get; set; }
        public string? VitesseScan { get; set; }

        // Écran
        public string? Taille { get; set; }
        public string? TypeEcran { get; set; }
        public string? TempsDeReponse { get; set; }

    }
}
