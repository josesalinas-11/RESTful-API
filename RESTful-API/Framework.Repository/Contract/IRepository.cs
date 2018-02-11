using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Framework.Repository.Contract
{
    public interface IRepository<TEntity> : IRepositoryReadOnly<TEntity>, IStoredProcedure
        where TEntity : class
    {

        void Insert(TEntity entity);

        void InsertRange(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        void Delete(Expression<Func<TEntity, bool>> predicate);

        #region Async

        Task<int> InsertAsync(TEntity entity);

        Task<int> InsertRangeAsync(IEnumerable<TEntity> entities);

        Task<int> UpdateAsync(TEntity entity);

        Task<int> DeleteAsync(TEntity entity);

        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

    }
}