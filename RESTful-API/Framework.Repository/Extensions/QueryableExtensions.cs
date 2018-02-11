using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Framework.Collections.Generic;
using Framework.PropertyMapper;

namespace Framework.Repository.Extensions
{
    public static class QueryableExtensions
    {
        public static PaginatedList<T> ToPaginatedList<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            if (query == null)
                throw new ArgumentException(nameof(query));

            var totalCount = query.Count();
            var collection = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return new PaginatedList<T>(collection, pageNumber, pageSize, totalCount);
        }

        public static IQueryable<T> Sort<T>(this IQueryable<T> source, string orderBy, Dictionary<string, PropertyMapperValue> propertyMapperValues)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (propertyMapperValues == null)
                throw new ArgumentNullException(nameof(propertyMapperValues));

            if (string.IsNullOrWhiteSpace(orderBy)) return source;

            var orderByAfterSplit = orderBy.Split(',');

            foreach (var orderByClause in orderByAfterSplit.Reverse())
            {
                var trimmedOrderByClause = orderByClause.Trim();

                var orderDescending = trimmedOrderByClause.EndsWith(" desc");

                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);

                if (!propertyMapperValues.ContainsKey(propertyName))
                    throw new ArgumentException($"Key mapping for {propertyName} is missing");

                var propertyMapperValue = propertyMapperValues[propertyName];

                if (propertyMapperValue == null)
                    throw new ArgumentNullException(nameof(propertyMapperValue));

                foreach (var destinationProperty in propertyMapperValue.DestinationProperties.Reverse())
                {
                    if (propertyMapperValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }
                    source = source.OrderBy(destinationProperty + (orderDescending ? " descending" : " ascending"));
                }
            }
            return source;
        }
    }
}