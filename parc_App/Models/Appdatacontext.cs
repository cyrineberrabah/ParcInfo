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
        }



    }



}
