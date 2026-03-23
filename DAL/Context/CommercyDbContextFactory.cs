using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Context
{
    public class CommercyDbContextFactory : IDesignTimeDbContextFactory<CommercyDbContext>
    {
        public CommercyDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CommercyDbContext>();
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=MiniECommercyDB;Trusted_Connection=True;TrustServerCertificate=True;");

            return new CommercyDbContext(optionsBuilder.Options);
        }
    }
}
