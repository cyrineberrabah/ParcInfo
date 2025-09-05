using Microsoft.EntityFrameworkCore;

namespace parc_App.Models
{
    public class Appdatacontext : DbContext
    {
        public Appdatacontext(DbContextOptions<Appdatacontext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Preneur> Preneurs { get; set; }
        public DbSet<Materiel> Materiels { get; set; }
        public DbSet<Departement> Departements { get; set; }

        public DbSet<HistoriqueAffectation> HistoriqueAffectations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Materiel>()
                .HasOne(m => m.Preneur)
                .WithMany(p => p.Materiels)
                .HasForeignKey(m => m.PreneurId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuration pour HistoriqueAffectation
            modelBuilder.Entity<HistoriqueAffectation>()
                .HasOne(h => h.Preneur)
                .WithMany() // ou .WithMany(p => p.HistoriqueAffectations) si tu ajoutes la collection
                .HasForeignKey(h => h.PreneurId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Preneur>()
                .HasOne(p => p.Departement)
                .WithMany(d => d.Preneurs)
                .HasForeignKey(p => p.DepartementId)
                .OnDelete(DeleteBehavior.Restrict); // ou Cascade si tu veux supprimer les preneurs quand le département est supprimé

        }



    }
}
