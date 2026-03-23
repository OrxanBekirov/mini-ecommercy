using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DAL
{
    public class CommercyDbContextFactory : IDesignTimeDbContextFactory<CommercyDbContext>
    {
        public CommercyDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CommercyDbContext>();

            // Bu hissə sadəcə miqrasiya yaransın deyə müvəqqəti bir "yol" göstərir.
            // Real bazaya qoşulmaq üçün hələ də Program.cs istifadə olunacaq.
            optionsBuilder.UseNpgsql("Host=localhost;Database=dummy;Username=postgres;Password=password");

            return new CommercyDbContext(optionsBuilder.Options);
        }
    }
}