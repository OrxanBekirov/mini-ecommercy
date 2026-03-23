using Core.DAL.BaseRepositories.Concrete.EntityFramework;
using DAL.Repositories.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Concrete
{
    public class EfPaymentrepository : EfBaseRepository<Payment, CommercyDbContext>, IPaymentRepository
    {
        public EfPaymentrepository(CommercyDbContext context) : base(context)
        {
        }
    }
}