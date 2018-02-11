using System;

namespace Framework.Data.Entity.Contract
{
    public interface ISoftDelete
    {
        DateTime DeleteAt { get; set; }
    }
}