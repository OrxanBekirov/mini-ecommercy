using Core.DAL.BaseRepositories.Concrete.EntityFramework;
using DAL.Repositories.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Concrete
{
    public class EfCartRepository : EfBaseRepository<Cart, CommercyDbContext>, ICartRepository
    {
        public EfCartRepository(CommercyDbContext context) : base(context)
        {
        }
    }
}
