using System.Collections.Generic;
using Framework.PropertyMapper.Contract;

namespace Framework.PropertyMapper
{
    public class PropertyMapper<TSource, TDestination> : IPropertyMapper
    {

        #region ReadOnly Properties

        public Dictionary<string, PropertyMapperValue> PropertyMapperValues { get; }

        #endregion

        #region Construct

        public PropertyMapper(Dictionary<string, PropertyMapperValue> propertyMapperValues)
        {
            PropertyMapperValues = propertyMapperValues;
        }

        #endregion

    }
}