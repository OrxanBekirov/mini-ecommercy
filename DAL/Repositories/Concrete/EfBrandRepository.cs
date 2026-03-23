using Core.DAL.BaseRepositories.Concrete.EntityFramework;
using DAL.Repositories.Abstract;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Concrete
{
    public class EfBrandRepository : EfBaseRepository<Brand, CommercyDbContext>, IBrandRepository
    {
        public EfBrandRepository(CommercyDbContext context) : base(context)
        {
        }
       

    }
}