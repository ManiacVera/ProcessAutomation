using Microsoft.EntityFrameworkCore;
using ProcessAutomation.Framework.DTO.Constants;
using ProcessAutomation.Framework.DTO.Info;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessAutomation.Framework.Base
{
    public sealed class BaseRepository
    {
        private static BaseRepository instance = null;
        private ConnectionInfo connectionInfo;
        private List<LibraryInfo> libraryList = new List<LibraryInfo>();

        public sealed class DataBaseType
        {
            public const string SQLSERVER = "SQLSERVER";
            public const string MYSQL = "MYSQL";
            public const string POSTGRES = "POSTGRES";
            public const string SQLITE = "SQLITE";
        }

        public IServiceProvider serviceProvider
        {
            get;
            private set;
        }

        public ContextUserInfo contextScopeInfo;

        private BaseRepository()
        {
            contextScopeInfo = new ContextUserInfo();
        }

        private string BuildConnectionString(string server, int port, string database, string username, string password, string databaseType)
        {
            string connectionString = null;

            switch (databaseType)
            {
                case DataBaseType.SQLSERVER:
                    connectionString = string.Format(FrameworkConstants.SQLSERVER_DATABASE_CONNECTION_FORMAT, server, port, database, username, password);
                    break;
                case DataBaseType.MYSQL:
                    connectionString = string.Format(FrameworkConstants.MYSQL_DATABASE_CONNECTION_FORMAT, server, port, database, username, password);
                    break;
                default:
                    connectionString = string.Format(FrameworkConstants.SQLSERVER_DATABASE_CONNECTION_FORMAT, server, port, database, username, password);
                    break;
            }


            return connectionString;
        }

        private int GetDefaultPort(string databaseType)
        {
            int port = 0;

            switch (databaseType)
            {
                case DataBaseType.SQLSERVER:
                    port = 1433;
                    break;
                case DataBaseType.MYSQL:
                    port = 3306;
                    break;
                default:
                    port = 1433;
                    break;
            }
            return port;
        }

        public static BaseRepository Create(string databaseType = DataBaseType.SQLSERVER)
        {
            if (instance == null)
            {
                instance = new BaseRepository();
            }
            return instance;
        }

        public static BaseRepository GetInstance()
        {
            return instance;
        }

        public void SetServiceProviderReadOnly(IServiceProvider serviceProvider)
        {
            if (this.serviceProvider == null)
            {
                this.serviceProvider = serviceProvider;

                foreach (LibraryInfo libraryInfo in libraryList)
                {
                    try
                    {
                        if (serviceProvider.GetService(libraryInfo.Type) != null)
                        {
                            libraryInfo.Library = serviceProvider.GetService(libraryInfo.Type);
                        }
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                    }
                }
            }
        }

        public void AddLibrary<TService, TImplementation>()
        {
            if (!libraryList.Any(f => f.Name == typeof(TService).Name))
            {
                libraryList.Add(new LibraryInfo
                {
                    Name = typeof(TService).Name,
                    Type = typeof(TService)
                });
            }
        }

        public T GetLibrary<T>()
        {
            T result = default(T);
            LibraryInfo libraryInfo = null;

            if ((libraryInfo = libraryList.FirstOrDefault(f => f.Name == typeof(T).Name)) != null)
            {
                result = (T)libraryInfo.Library;
            }

            return result;
        }

        public T GetLibrary<T>(string libraryName)
        {
            T result = default(T);
            LibraryInfo libraryInfo = null;

            if ((libraryInfo = libraryList.FirstOrDefault(f => f.Name == libraryName)) != null)
            {
                result = (T)libraryInfo.Library;
            }

            return result;
        }

        public object GetLibrary(string libraryName)
        {
            return GetLibrary<object>(libraryName);
        }

        public DbContextOptions<T> GetDbContextOptions<T>(string connectionString, string databaseType) where T : DbContext
        {
            DbContextOptionsBuilder<T> optionsBuilder = new DbContextOptionsBuilder<T>();

            switch (databaseType)
            {
                case DataBaseType.SQLSERVER:
                    optionsBuilder.UseSqlServer(connectionString);
                    break;
                case DataBaseType.MYSQL:
                    //optionsBuilder.UseMySQL(connectionString);
                    break;
                default:
                    optionsBuilder.UseSqlServer(connectionString);
                    break;
            }
            return optionsBuilder.Options;
        }

        public DbContextOptions<T> GetDbContextOptions<T>(string server, int port, string database, string username, string password, string databaseType) where T : DbContext
        {
            if (port == 0)
            {
                port = GetDefaultPort(databaseType);
            }

            return GetDbContextOptions<T>(BuildConnectionString(server, port, database, username, password, databaseType), databaseType);
        }

        public DbContextOptions<T> GetDbContextOptions<T>(ConnectionInfo connectionInfo) where T : DbContext
        {
            DbContextOptions<T> contextOptions = null; ;

            if (connectionInfo != null)
            {
                if (connectionInfo.Port == 0)
                {
                    connectionInfo.Port = GetDefaultPort(connectionInfo.DatabaseType);
                }

                contextOptions = GetDbContextOptions<T>(BuildConnectionString(connectionInfo.Server, connectionInfo.Port
                                                        , connectionInfo.Database, connectionInfo.UserName, connectionInfo.Password
                                                                              , connectionInfo.DatabaseType), connectionInfo.DatabaseType);
            }

            return contextOptions;
        }

        public DbContextOptions<T> GetCommonDbContextOptionsAndChangeConnection<T>(string server, int port, string database, string username, string password, string databaseType) where T : DbContext
        {
            ConnectionInfo connnectionInfo = new ConnectionInfo();

            if (server != null && !string.IsNullOrEmpty(server))
            {
                connnectionInfo.Server = server.Trim();
            }
            else
            {
                connnectionInfo.Server = this.connectionInfo.Server.Trim();
            }

            if (port != 0)
            {
                connnectionInfo.Port = port;
            }
            else
            {
                connnectionInfo.Port = this.connectionInfo.Port;
            }

            if (database != null && !string.IsNullOrEmpty(database.Trim()))
            {
                connnectionInfo.Database = database.Trim();
            }
            else
            {
                connnectionInfo.Database = this.connectionInfo.Database;
            }

            if (username != null && !string.IsNullOrEmpty(username.Trim()))
            {
                connnectionInfo.UserName = username.Trim();
            }
            else
            {
                connnectionInfo.UserName = this.connectionInfo.UserName;
            }

            if (password != null && !string.IsNullOrEmpty(password.Trim()))
            {
                connnectionInfo.Password = password.Trim();
            }
            else
            {
                connnectionInfo.Password = this.connectionInfo.Password.Trim();
            }

            if (databaseType != null && !string.IsNullOrEmpty(databaseType.Trim()))
            {
                connnectionInfo.DatabaseType = databaseType.Trim();
            }
            else
            {
                connnectionInfo.DatabaseType = this.connectionInfo.DatabaseType.Trim();
            }

            return GetDbContextOptions<T>(BuildConnectionString(server, port, database, username, password, databaseType), databaseType);
        }

        // Luis Vera, debe de ser internal
        public void SetCommonConnection(ConnectionInfo connectionInfo)
        {
            this.connectionInfo = connectionInfo;
        }

        // internal
        public ConnectionInfo GetCommonConnection()
        {
            return connectionInfo;
        }
    }
}
