namespace Framework.Data.Entity.Contract
{
    public interface IConcurrency
    {
        byte[] RowVersion { get; set; }
    }
}