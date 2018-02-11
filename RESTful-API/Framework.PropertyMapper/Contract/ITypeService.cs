namespace Framework.PropertyMapper.Contract
{
    public interface ITypeService
    {
        bool TypeHasProperties<T>(string fields);
    }
}