using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Framework.Collections.Generic;
using Framework.PropertyMapper;

namespace Framework.Repository.Contract
{
    public interface IRepositoryReadOnly<TEntity> where TEntity : class
    {
        PaginatedList<TEntity> Paginate(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> predicate, string orderBy, Dictionary<string, PropertyMapperValue> propertyMappingValues, bool showDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties);

        PaginatedList<TResult> Paginate<TResult>(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> predicate, Func<TEntity, TResult> selector, string orderBy, Dictionary<string, PropertyMapperValue> propertyMappingValues, bool showDeleted, params Expression<Func<TEntity, object>>[] includeProperties);

        TEntity GetSingle(Expression<Func<TEntity, bool>> predicate, bool showDeleted = false);

        TResult GetSingle<TResult>(Expression<Func<TEntity, bool>> predicate, Func<TEntity, TResult> selector, bool showDeleted = false) where TResult : class;
    }
}