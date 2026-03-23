using Core.DAL.BaseRepositories.Absrtact;
using Core.DAL.BaseRepositories.Concrete.EntityFramework;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Abstract
{
    public interface IProductRepository:IBaseRepository<Product>
    {
       
    }
}
