using System.Data.Common;

namespace Framework.Repository.Contract
{
    public interface IStoredProcedure
    {
        DbCommand StoredProcedure(string name, bool defaultSchema = true);
    }
}