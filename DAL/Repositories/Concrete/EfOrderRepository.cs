using Core.DAL.BaseRepositories.Concrete.EntityFramework;
using DAL.Repositories.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Concrete
{
    public class EfOrderRepository : EfBaseRepository<Order, CommercyDbContext>, IOrderRepository
    {
        public EfOrderRepository(CommercyDbContext context) : base(context)
        {
        }
    }
}