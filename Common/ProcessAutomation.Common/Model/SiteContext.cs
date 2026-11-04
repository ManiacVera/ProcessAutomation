using Microsoft.EntityFrameworkCore;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Framework.DTO.Constants;
using ProcessAutomation.Framework.DTO.Enum;
using ProcessAutomation.Framework.DTO.Info;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ProcessAutomation.Common.Model
{
    //public enum DataBaseTypeEnum 
    //{
    //    SQLITE = 1,
    //    SQLSERVER = 2
    //}

    public class SiteContext : DbContext
    {
        private string connectionString = null;
        private DatabaseTypeEnum dataBaseTypeEnum = DatabaseTypeEnum.SQLSERVER;
        private static SiteContext instance;
        public const string database = "PROC_AUTO_STORE.db";
        
        public DbSet<CAT_DEVICE> CAT_DEVICE { get; set; }        
        public DbSet<CAT_STATUS> CAT_STATUS { get; set; }
        public DbSet<CAT_STATUS_TYPE> CAT_STATUS_TYPE { get; set; }
        public DbSet<CAT_TOKEN_TYPE> CAT_TOKEN_TYPE { get; set; }
        public DbSet<CAT_USER> CAT_USER { get; set; }
        //public DbSet<CHAT> CHAT { get; set; }
        //public DbSet<CHAT_MESSAGE> CHAT_MESSAGE { get; set; }
        //public DbSet<CHAT_USER_MESSAGE> CHAT_USER_MESSAGE { get; set; }
        public DbSet<MESSAGE_ADMIN> MESSAGE_ADMIN { get; set; }
        public DbSet<TOKEN> TOKEN { get; set; }
        public DbSet<USER_LOGIN> USER_LOGIN { get; set; }

//#if DISABLE_FUNCTIONS
        private static SiteContext GetContext()
        {
            if (instance == null || !instance.Database.CanConnect())
            {
                CreateStaticContext(instance.connectionString, instance.dataBaseTypeEnum);
            }

            return instance;
        }

        public static void UsingContext(Action<SiteContext> function)
        {
            if (function != null)
            {
                SiteContext context = GetContext();
                function(context);
                CloseConnection();
            }
        }

        private static void CloseConnection()
        {
            if (instance != null)
            {
                if (instance.Database.CanConnect())
                {
                    instance.Database.CloseConnection();
                }
                //instance.Dispose();
                //instance = null;
            }
        }
//#endif //DISABLE_FUNCTIONS

        private static string BuildStringConnection(DatabaseTypeEnum databaseType, string server, string database, 
            int port = 0, string userName = null, string password = null)
        {
            string connectionString = null;
            switch (databaseType)
            {
                case DatabaseTypeEnum.SQLSERVER:
                    connectionString = string.Format(FrameworkConstants.SQLSERVER_DATABASE_CONNECTION_FORMAT, server
                        , port, database, userName, password);
                    break;

                case DatabaseTypeEnum.MYSQL:
                    connectionString = string.Format(FrameworkConstants.MYSQL_DATABASE_CONNECTION_FORMAT, server
                        , port, database, userName, password);
                    break;

                case DatabaseTypeEnum.SQLITE:
                    connectionString = string.Format(FrameworkConstants.SQLITE_DATABASE_CONNECTION_FORMAT, server, database);
                    break;
            }
            return connectionString;
        }

        public static SiteContext CreateContext(ConnectionInfo connectionInfo)
        {
            SiteContext instance = null;
            if (connectionInfo != null)
            {
                DatabaseTypeEnum databaseType = (DatabaseTypeEnum)Enum.Parse(typeof(DatabaseTypeEnum), connectionInfo.DatabaseType);
                string connectionString = BuildStringConnection(databaseType, connectionInfo.Server, connectionInfo.Database, connectionInfo.Port
                    , connectionInfo.UserName, connectionInfo.Password);

                instance = new SiteContext(connectionString, databaseType);
                instance.connectionString = connectionString;
                instance.dataBaseTypeEnum = databaseType;
                //instance.Database.EnsureCreated();
            }
            return instance;
        }

        public static SiteContext CreateContext(string connectionString, DatabaseTypeEnum dataBaseTypeEnum)
        {
            SiteContext instance = null;

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                instance = new SiteContext(connectionString, dataBaseTypeEnum);
                instance.connectionString = connectionString;
                instance.dataBaseTypeEnum = dataBaseTypeEnum;
                //instance.Database.EnsureCreated();
            }

            return instance;
        }

        public static SiteContext CreateStaticContext(ConnectionInfo connectionInfo)
        {
            if (connectionInfo != null && instance == null)
            {
                DatabaseTypeEnum databaseType = (DatabaseTypeEnum)Enum.Parse(typeof(DatabaseTypeEnum), connectionInfo.DatabaseType);
                string connectionString = BuildStringConnection(databaseType, connectionInfo.Server, connectionInfo.Database, connectionInfo.Port
                    , connectionInfo.UserName, connectionInfo.Password);

                instance = new SiteContext(connectionString, databaseType);
                instance.connectionString = connectionString;
                instance.dataBaseTypeEnum = databaseType;
                //instance.Database.EnsureCreated();
            }
            return instance;
        }

        public static SiteContext CreateStaticContext(string connectionString, DatabaseTypeEnum dataBaseTypeEnum)
        {
            if (instance == null && !string.IsNullOrWhiteSpace(connectionString))
            {
                instance = new SiteContext(connectionString, dataBaseTypeEnum);
                instance.connectionString = connectionString;
                instance.dataBaseTypeEnum = dataBaseTypeEnum;
                //instance.Database.EnsureCreated();
            }
            return instance;
        }

        //        private SiteContext(string connectionString)
        //            : base(GetOptions(connectionString))
        //        {
        //        }

        //        private static DbContextOptions GetOptions(string connectionString)
        //        {
        //            return SqliteDbContextOptionsBuilderExtensions.UseSqlite(new DbContextOptionsBuilder(), connectionString).Options;
        //        }

        private SiteContext(string connectionString, DatabaseTypeEnum dataBaseTypeEnum)
            : base(GetOptions(connectionString, dataBaseTypeEnum))
        {
        }

        private static DbContextOptions GetOptions(string connectionString, DatabaseTypeEnum dataBaseTypeEnum)
        {
            DbContextOptions options = null;

            switch (dataBaseTypeEnum)
            {
                case DatabaseTypeEnum.SQLITE:
                    options = SqliteDbContextOptionsBuilderExtensions.UseSqlite(new DbContextOptionsBuilder(), connectionString).Options;
                    break;
                default:
                    options = SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
                    break;
            }

            return options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Blog>()
            //   .HasKey(b => b.BlogId)
            //   .HasName("PrimaryKey_BlogId");
        }

        public static void BuiderCommonDatabase()
        {
            UsingContext(instance =>
            {
                string location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Assembly commonAssembly = Assembly.LoadFrom(Path.Combine(location, CommonConstants.PACKAGE_FOLDER, string.Format("{0}.dll", CommonConstants.COMMON_NAMESPACE)));
                string sqlFolder = string.Concat(string.Join(".", commonAssembly.GetName().Name, CommonConstants.RESOURCES_FOLDER, CommonConstants.DATABASE_FOLDER), ".");

                List<string> scriptFileList = commonAssembly.GetManifestResourceNames().Where(s => s.ToUpper().Contains(sqlFolder.ToUpper()))
                    .Select(s => s.Replace(sqlFolder, string.Empty)).ToList();

                switch (instance.dataBaseTypeEnum)
                {
                    case DatabaseTypeEnum.SQLSERVER:
                        scriptFileList = scriptFileList.Where(f => f.StartsWith("S_") && f.EndsWith(".sql")).OrderBy(s => Convert.ToDecimal(s.Split('_')[1])).ToList();
                        break;

                    case DatabaseTypeEnum.SQLITE:
                        scriptFileList = scriptFileList.Where(f => f.StartsWith("S_") && f.EndsWith(".sqlite")).OrderBy(s => Convert.ToDecimal(s.Split('_')[1])).ToList();
                        break;

                    default:
                        scriptFileList = scriptFileList.Where(f => f.StartsWith("S_") && f.EndsWith(".sqlite")).OrderBy(s => Convert.ToDecimal(s.Split('_')[1])).ToList();
                        break;
                }

                if (scriptFileList.Count > 0)
                {
                    foreach (string sqlFile in scriptFileList)
                    {
                        using (Stream stream = commonAssembly.GetManifestResourceStream(string.Concat(sqlFolder, sqlFile)))
                        {
                            if (stream != null)
                            {
                                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                                {
                                    //string[] scriptArray = Regex.Split(reader.ReadToEnd(), @"^\w+GO$", RegexOptions.Multiline);
                                    //foreach (string script in scriptArray)
                                    //{
                                    //    instance.Database.ExecuteSqlRaw(script);
                                    //}
                                    instance.Database.ExecuteSqlRaw(reader.ReadToEnd());
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}
