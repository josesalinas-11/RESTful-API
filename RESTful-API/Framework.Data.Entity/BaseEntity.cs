using System;
using Framework.Data.Entity.Contract;

namespace Framework.Data.Entity
{
    public abstract class BaseEntity : ISoftDelete, IConcurrency
    {

        #region ISofDelete

        public DateTime DeleteAt { get; set; }

        #endregion

        #region IConcurrency

        public byte[] RowVersion { get; set; }

        #endregion

    }
}
