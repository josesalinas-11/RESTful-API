using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Framework.Collections.Generic;

namespace Framework.Repository.Extensions
{
    public static class EnumerableExtensions
    {
        public static PaginatedList<T> ToPaginatedList<T>(this IEnumerable<T> query, int pageNumber, int pageSize)
        {
            if (query == null)
                throw new ArgumentException(nameof(query));

            var enumerable = query.ToList();
            var totalCount = enumerable.Count;
            var collection = enumerable.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return new PaginatedList<T>(collection, pageNumber, pageSize, totalCount);
        }

        public static IEnumerable<ExpandoObject> ShapeToEnumerable<TSource>(this IEnumerable<TSource> source, string fields)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var expandoObjectList = new List<ExpandoObject>();

            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                var fieldsAfterSplit = fields.Split(',');

                foreach (var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();

                    var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                        throw new Exception($"Property {propertyName} wasn't found on {typeof(TSource)}");

                    propertyInfoList.Add(propertyInfo);
                }
            }

            foreach (var sourceObject in source)
            {
                var dataShapedObject = new ExpandoObject();

                foreach (var propertyInfo in propertyInfoList)
                {
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }

                expandoObjectList.Add(dataShapedObject);
            }

            return expandoObjectList;
        }
    }
}