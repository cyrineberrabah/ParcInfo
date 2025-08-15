using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using parc_App.Models;

public class AppdatacontextFactory : IDesignTimeDbContextFactory<Appdatacontext>
{
    public Appdatacontext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<Appdatacontext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=parcinfo_dbc;Trusted_Connection=True;TrustServerCertificate=True;");

        return new Appdatacontext(optionsBuilder.Options);
    }
}
