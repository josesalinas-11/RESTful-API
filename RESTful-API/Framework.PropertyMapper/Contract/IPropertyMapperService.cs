using System.Collections.Generic;

namespace Framework.PropertyMapper.Contract
{
    public interface IPropertyMapperService
    {
        bool ValidMappingExistsFor<TSource, TDestination>(string fields);

        Dictionary<string, PropertyMapperValue> GetPropertyMapper<TSource, TDestination>();
    }
}