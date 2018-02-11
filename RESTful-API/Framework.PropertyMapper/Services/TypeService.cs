using System.Reflection;
using Framework.PropertyMapper.Contract;

namespace Framework.PropertyMapper.Services
{
    public class TypeService : ITypeService
    {

        #region ITypeService

        public bool TypeHasProperties<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields)) return true;

            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();

                var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null) return false;
            }

            return true;
        }

        #endregion

    }
}