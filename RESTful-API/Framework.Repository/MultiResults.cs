using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Framework.Repository.Extensions;

namespace Framework.Repository
{
    public class MultiResults
    {
        private readonly DbDataReader _dataReader;

        public MultiResults(DbDataReader dataReader)
        {
            _dataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
        }

        public IList<T> ReadToList<T>()
        {
            return _dataReader.MapToList<T>();
        }

        public T? ReadToValue<T>() where T : struct
        {
            return _dataReader.MapToValue<T>();
        }

        public Task<bool> NextResultAsync()
        {
            return _dataReader.NextResultAsync();
        }

        public Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            return _dataReader.NextResultAsync(cancellationToken);
        }

        public bool NextResult()
        {
            return _dataReader.NextResult();
        }
    }
}