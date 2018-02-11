using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Framework.Repository.Extensions
{
    public static class DbDataReaderExtensions
    {
        public static IList<T> MapToList<T>(this DbDataReader dataReader)
        {
            if (dataReader == null) throw new ArgumentNullException(nameof(dataReader));

            var list = new List<T>();
            var properties = typeof(T).GetRuntimeProperties();

            var columns = dataReader.GetColumnSchema()
                .Where(x => properties.Any(y =>
                    string.Equals(y.Name, x.ColumnName, StringComparison.CurrentCultureIgnoreCase)))
                .ToDictionary(key => key.ColumnName);

            if (!dataReader.HasRows) return list;

            var propertyInfos = properties as IList<PropertyInfo> ?? properties.ToList();
            while (dataReader.Read())
            {
                var instance = Activator.CreateInstance<T>();
                foreach (var propertyInfo in propertyInfos)
                {
                    var columnOrdinal = columns[propertyInfo.Name].ColumnOrdinal;
                    if (columnOrdinal == null) continue;
                    var val = dataReader.GetValue(columnOrdinal.Value);
                    propertyInfo.SetValue(instance, val == DBNull.Value ? null : val);
                }
                list.Add(instance);
            }
            return list;
        }

        public static T? MapToValue<T>(this DbDataReader dataReader) where T : struct
        {
            if (dataReader == null) throw new ArgumentNullException(nameof(dataReader));

            if (!dataReader.HasRows) return new T?();

            if (dataReader.Read())
                return dataReader.IsDBNull(0) ? new T?() : dataReader.GetFieldValue<T>(0);

            return new T?();
        }
    }
}