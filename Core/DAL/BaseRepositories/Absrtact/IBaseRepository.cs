using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.DAL.BaseRepositories.Absrtact
{
    public interface IBaseRepository<TEntity>
     where TEntity : class
    {
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null, params string[] includes);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter, params string[] includes);
        //Pagination
        Task<List<TEntity>> GetAllWithPaginationAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> filter, params string[] includes);
        Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> filter);
        Task AddAsync(TEntity entity);
        public void Remove(TEntity entity);
        public IQueryable<TEntity> GetQuery(string[] includes);
        public void Update(TEntity entity);
    }
}
