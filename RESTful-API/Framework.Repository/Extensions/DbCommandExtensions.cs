using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Repository.Extensions
{
    public static class DbCommandExtensions
    {
        public static DbCommand WithParameter(this DbCommand command, string name, object value, Action<DbParameter> configure = null)
        {
            if (string.IsNullOrEmpty(command.CommandText) && command.CommandType != CommandType.StoredProcedure)
                throw new InvalidOperationException("Call StoredProcedure before using this method");

            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            configure?.Invoke(parameter);
            command.Parameters.Add(parameter);
            return command;
        }

        public static DbCommand WithParameter(this DbCommand command, string name, Action<DbParameter> configure = null)
        {
            if (string.IsNullOrEmpty(command.CommandText) && command.CommandType != CommandType.StoredProcedure)
                throw new InvalidOperationException("Call StoredProcedure before using this method");

            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            configure?.Invoke(parameter);
            command.Parameters.Add(parameter);
            return command;
        }

        public static IList<T> Call<T>(this DbCommand command)
        {
            using (command)
            {
                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();
                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        return reader.MapToList<T>();
                    }
                }
                finally
                {
                    command.Connection.Close();
                }
            }
        }

        public static void Call(this DbCommand command, Action<MultiResults> results, CommandBehavior commandBehaviour = CommandBehavior.Default)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));

            using (command)
            {
                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();
                try
                {
                    using (var reader = command.ExecuteReader(commandBehaviour))
                    {
                        var result = new MultiResults(reader);
                        results(result);
                    }
                }
                finally
                {
                    command.Connection.Close();
                }
            }
        }

        public static async Task CallAsync(this DbCommand command, Action<MultiResults> results, CommandBehavior commandBehaviour = CommandBehavior.Default, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (results == null) throw new ArgumentNullException(nameof(results));

            using (command)
            {
                if (command.Connection.State == ConnectionState.Closed)
                    await command.Connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    using (var reader = await command.ExecuteReaderAsync(commandBehaviour, cancellationToken).ConfigureAwait(false))
                    {
                        var result = new MultiResults(reader);
                        results(result);
                    }
                }
                finally
                {
                    command.Connection.Close();
                }
            }
        }

    }
}