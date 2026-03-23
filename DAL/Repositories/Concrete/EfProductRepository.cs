using Core.DAL.BaseRepositories.Concrete.EntityFramework;
using DAL.Repositories.Abstract;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Concrete
{
    public class EfProductRepository : EfBaseRepository<Product, CommercyDbContext>, IProductRepository
    {
        public EfProductRepository(CommercyDbContext context) : base(context)
        {
        }

        
    }
}
