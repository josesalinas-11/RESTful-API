using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Framework.EntityFrameworkRepository.Extensions
{
    #region LoggingCategories

    public enum LoggingCategories
    {
        All = 0,
        Sql = 1
    }

    #endregion

    #region DbContextLoggingExtensions

    public static class DbContextLoggingExtensions
    {
        public static void ConfigureLogging(this DbContext db, Action<string> logger, Func<string, LogLevel, bool> filter)
        {
            var serviceProvider = db.GetInfrastructure();
            var loggerFactory = (ILoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory));

            LogProvider.CreateOrModifyLoggerForDbContext(db.GetType(), loggerFactory, logger, filter);
        }

        public static void ConfigureLogging(this DbContext db, Action<string> logger, LoggingCategories categories = LoggingCategories.All)
        {
            var serviceProvider = db.GetInfrastructure();
            var loggerFactory = (LoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory));

            switch (categories)
            {
                case LoggingCategories.Sql:
                    var sqlCategories = new List<string> { DbLoggerCategory.Database.Command.Name,
                        DbLoggerCategory.Query.Name,
                        DbLoggerCategory.Update.Name };
                    LogProvider.CreateOrModifyLoggerForDbContext(db.GetType(),
                        loggerFactory,
                        logger,
                        (c, l) => sqlCategories.Contains(c));
                    break;
                case LoggingCategories.All:
                    LogProvider.CreateOrModifyLoggerForDbContext(db.GetType(),
                        loggerFactory, logger,
                        (c, l) => true);
                    break;
            }
        }
    }

    #endregion

    #region LogProvider

    internal class LogProvider : ILoggerProvider
    {

        public volatile LoggingConfiguration Configuration;
        private static bool DefaultFilter(string categoryName, LogLevel level) => true;
        private static readonly ConcurrentDictionary<Type, LogProvider> Providers = new ConcurrentDictionary<Type, LogProvider>();

        public static void CreateOrModifyLoggerForDbContext(Type dbContextType, ILoggerFactory loggerFactory, Action<string> logger, Func<string, LogLevel, bool> filter = null)
        {
            var isNew = false;
            var provider = Providers.GetOrAdd(dbContextType, t =>
            {
                using (var p = new LogProvider(logger, filter ?? DefaultFilter))
                {
                    loggerFactory.AddProvider(p);
                    isNew = true;
                    return p;
                }
            });

            if (!isNew)
            {
                provider.Configuration = new LoggingConfiguration(logger, filter ?? DefaultFilter);
            }

        }

        public class LoggingConfiguration
        {
            public LoggingConfiguration(Action<string> logger, Func<string, LogLevel, bool> filter)
            {
                Logger = logger;
                Filter = filter;
            }
            public readonly Action<string> Logger;
            public readonly Func<string, LogLevel, bool> Filter;
        }


        private LogProvider(Action<string> logger, Func<string, LogLevel, bool> filter)
        {
            Configuration = new LoggingConfiguration(logger, filter);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(categoryName, this);
        }

        public void Dispose()
        { }

        private class Logger : ILogger
        {

            private readonly string _categoryName;
            private readonly LogProvider _provider;
            public Logger(string categoryName, LogProvider provider)
            {
                _provider = provider;
                _categoryName = categoryName;
            }
            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                var config = _provider.Configuration;
                if (config.Filter(_categoryName, logLevel))
                {
                    config.Logger(formatter(state, exception));
                }
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }
    }

    #endregion

}