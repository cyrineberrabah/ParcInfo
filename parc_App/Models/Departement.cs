namespace parc_App.Models
{
    public class Departement
    {
        public int Id { get; set; }
        public string Nom { get; set; }

        public ICollection<Preneur> Preneurs { get; set; } = new List<Preneur>();


    }

}
