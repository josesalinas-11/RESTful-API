using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Framework.Collections.Generic;
using Framework.EntityFrameworkRepository.Context;
using Framework.PropertyMapper;
using Framework.Repository.Contract;
using Framework.Repository.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Framework.EntityFrameworkRepository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        #region Fields

        private readonly DataContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        #endregion

        #region Constructor

        public Repository(DataContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = dbContext.Set<TEntity>();
        }

        #endregion

        #region IRepository

        public PaginatedList<TEntity> Paginate(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> predicate, string orderBy, Dictionary<string, PropertyMapperValue> propertyMapperValues, bool showDeleted, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var queryable = GetAllIncluding(predicate, showDeleted, includeProperties).Sort(orderBy, propertyMapperValues);
            return queryable.ToPaginatedList(pageNumber, pageSize);
        }

        public PaginatedList<TResult> Paginate<TResult>(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> predicate, Func<TEntity, TResult> selector, string orderBy, Dictionary<string, PropertyMapperValue> propertyMapperValues, bool showDeleted, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var queryable = GetAllIncluding(predicate, showDeleted, includeProperties).Select(selector).AsQueryable().Sort(orderBy, propertyMapperValues);
            return queryable.ToPaginatedList(pageNumber, pageSize);
        }

        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
            _dbContext.SaveChanges();
        }

        public void InsertRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
            _dbContext.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = GetSingle(predicate);
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }

        #endregion

        #region IRepositoryAsync

        public Task<int> InsertAsync(TEntity entity)
        {
            _dbSet.AddAsync(entity);
            return _dbContext.SaveChangesAsync();
        }

        public Task<int> InsertRangeAsync(IEnumerable<TEntity> entity)
        {
            _dbSet.AddRangeAsync(entity);
            return _dbContext.SaveChangesAsync();
        }

        public Task<int> UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            return _dbContext.SaveChangesAsync();
        }

        public Task<int> DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            return _dbContext.SaveChangesAsync();
        }

        public Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = GetSingle(predicate);
            _dbSet.Remove(entity);
            return _dbContext.SaveChangesAsync();
        }

        #endregion

        #region IRepositoryReadOnly

        public TEntity GetSingle(Expression<Func<TEntity, bool>> predicate, bool showDeleted = false)
        {
            var entities = showDeleted
                ? _dbSet.AsNoTracking()
                : _dbSet.AsNoTracking().IgnoreQueryFilters().Where(w => EF.Property<DateTime?>(w, "DeletedOn") == null);
            return entities.SingleOrDefault(predicate);
        }

        public TResult GetSingle<TResult>(Expression<Func<TEntity, bool>> predicate, Func<TEntity, TResult> selector, bool showDeleted = false) where TResult : class
        {
            var entities = showDeleted
                ? _dbSet.AsNoTracking().AsEnumerable().Select(selector)
                : _dbSet.AsNoTracking().IgnoreQueryFilters().Where(w => EF.Property<DateTime?>(w, "DeletedOn") == null).Where(predicate).Select(selector);
            return entities.FirstOrDefault();
        }

        public TEntity GetSingleIncluding(Expression<Func<TEntity, bool>> predicate, bool showDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = GetAllIncluding(showDeleted, includeProperties);
            return entities.SingleOrDefault(predicate);
        }

        public TResult GetSingleIncluding<TResult>(Expression<Func<TEntity, bool>> predicate, Func<TEntity, TResult> selector, bool showDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties) where TResult : class
        {
            var entities = GetAllIncluding(showDeleted, includeProperties);
            return entities.Where(predicate).Select(selector).FirstOrDefault();
        }

        #endregion

        #region IStoredProcedure

        public DbCommand StoredProcedure(string name, bool defaultSchema = true)
        {
            var command = _dbContext.Database.GetDbConnection().CreateCommand();
            if (defaultSchema)
            {
                var schemaName = _dbContext.Model.Relational().DefaultSchema;
                if (schemaName != null)
                {
                    name = $"{schemaName}.{name}";
                }
            }
            command.CommandText = name;
            command.CommandType = CommandType.StoredProcedure;
            return command;
        }

        #endregion 

        #region Private Methods

        private IQueryable<TEntity> GetAllIncluding(bool showDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var queryable = showDeleted
                ? _dbSet.AsNoTracking()
                : _dbSet.AsNoTracking().IgnoreQueryFilters().Where(w => EF.Property<DateTime?>(w, "DeletedOn") == null);
            return includeProperties.Aggregate(queryable, (current, includeProperty) => current.Include(includeProperty));
        }

        private IQueryable<TEntity> GetAllIncluding(Expression<Func<TEntity, bool>> predicate = null, bool showDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var queryable = showDeleted
                ? (predicate == null ? _dbSet.AsNoTracking() : _dbSet.AsNoTracking().Where(predicate))
                : (predicate == null ? _dbSet.AsNoTracking().IgnoreQueryFilters().Where(w => EF.Property<DateTime?>(w, "DeletedOn") == null) : _dbSet.AsNoTracking().IgnoreQueryFilters().Where(w => EF.Property<DateTime?>(w, "DeletedOn") == null).Where(predicate));
            queryable = predicate != null ? queryable.Where(predicate).AsQueryable() : queryable;
            return includeProperties.Aggregate(queryable, (current, includeProperty) => current.Include(includeProperty)).AsQueryable();
        }

        #endregion

    }
}