using Core.DAL.BaseRepositories.Concrete.EntityFramework;
using DAL.Repositories.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Concrete
{
    public class EfCartItemRepository : EfBaseRepository<CartItem, CommercyDbContext>, ICartItemRepository
    {
        public EfCartItemRepository(CommercyDbContext context) : base(context)
        {
        }
    }
}