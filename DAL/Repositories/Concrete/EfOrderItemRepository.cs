using Core.DAL.BaseRepositories.Concrete.EntityFramework;
using DAL.Repositories.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Concrete
{
    public class EfOrderItemRepository : EfBaseRepository<OrderItem, CommercyDbContext>, IOrderItemRepository
    {
        public EfOrderItemRepository(CommercyDbContext context) : base(context)
        {
        }
    }
}