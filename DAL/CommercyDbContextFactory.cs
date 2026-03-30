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
            // 1. appsettings.json faylını oxumaq üçün konfiqurasiya qurulur
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Adətən WebApi qovluğunu göstərməlidir
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<CommercyDbContext>();

            // 2. Connection string-i fayldan oxuyuruq
            // Əgər tapmasa, birbaşa Railway string-ini bura yapışdıra da bilərsiniz (müvəqqəti test üçün)
            var connectionString = configuration.GetConnectionString("DefaultConnection");
                                   

            optionsBuilder.UseNpgsql(connectionString);

            return new CommercyDbContext(optionsBuilder.Options);
        }
    }
}