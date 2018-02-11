using System.Collections.Generic;

namespace Framework.PropertyMapper
{
    public class PropertyMapperValue
    {

        #region ReadOnly Properties

        public IEnumerable<string> DestinationProperties { get; }
        public bool Revert { get; }

        #endregion

        #region Construct

        public PropertyMapperValue(IEnumerable<string> destinationProperties, bool revert = false)
        {
            DestinationProperties = destinationProperties;
            Revert = revert;
        }

        #endregion

    }
}