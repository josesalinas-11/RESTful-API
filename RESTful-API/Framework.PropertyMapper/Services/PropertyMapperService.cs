using System;
using System.Collections.Generic;
using System.Linq;
using Framework.PropertyMapper.Contract;

namespace Framework.PropertyMapper.Services
{
    public abstract class PropertyMapperService : IPropertyMapperService
    {

        #region Protected Fields

        protected IList<IPropertyMapper> PropertyMappers;

        #endregion

        #region IPropertyMapperService

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapper = GetPropertyMapper<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields)) return true;

            var fieldsAfterSplit = fields.Split(',');

            return (from field in fieldsAfterSplit select field.Trim() into trimmedField let indexOfFirstSpace = trimmedField.IndexOf(" ", StringComparison.Ordinal) select indexOfFirstSpace == -1 ? trimmedField : trimmedField.Remove(indexOfFirstSpace)).All(propertyName => propertyMapper.ContainsKey(propertyName));
        }

        public Dictionary<string, PropertyMapperValue> GetPropertyMapper<TSource, TDestination>()
        {
            var propertyMappers = PropertyMappers.OfType<PropertyMapper<TSource, TDestination>>();

            var mappers = propertyMappers.ToList();
            if (mappers.Count == 1)
            {
                return mappers.First().PropertyMapperValues;
            }

            throw new Exception($"Cannot find exact property mapper instance for <{typeof(TSource)},{typeof(TDestination)}>");
        }

        #endregion
    }
}