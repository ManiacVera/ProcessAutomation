using Microsoft.EntityFrameworkCore;
// using ProcessAutomation.Framework.DTO.Interface;
using ProcessAutomationMAUI.Base;
using ProcessAutomationMAUI.DTO.Constants;
using System.ComponentModel;
using System;
using System.Formats.Tar;

namespace ProcessAutomationMAUI.DataAccess
{
    public class StoreDbContext : DbContext
    {
        private readonly string databasePath = null;
        public const int minutesRefreshData = 1440;
        public DbSet<CAT_COMPONENT> CAT_COMPONENT { get; set; }
        public DbSet<CAT_ENCAPSULATED_PARTITION_TYPE> CAT_ENCAPSULATED_PARTITION_TYPE { get; set; }
        public DbSet<CAT_VALUE_TYPE> CAT_VALUE_TYPE { get; set; }
        public DbSet<CAT_VALUE_OUTPUT_TYPE> CAT_VALUE_OUTPUT_TYPE { get; set; }
        public DbSet<CAT_DECLARATION_TYPE> CAT_DECLARATION_TYPE { get; set; }
        public DbSet<CAT_FILE> CAT_FILE { get; set; }
        public DbSet<SEC_TABLE_CONTROL> SEC_TABLE_CONTROL { get; set; }

        public StoreDbContext()
        {
            databasePath = Path.Combine(Utilities.GetApplicationPath(), ApplicationConstants.STORE_DATABASE_FILE);
        }

        #if __IOS__ || __MACCATALYST__

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string conexionDb = $"Filename={databasePath}";

            optionsBuilder
                .UseModel(DbContextModel.Instance)
                .UseSqlite(conexionDb);
        }
        #else

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string conexionDb = $"Filename={databasePath}";
            optionsBuilder.UseSqlite(conexionDb);
        }
        
        #endif

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<CAT_COMPONENT>(entity =>
            //{
            //    entity.HasKey(c => c.IdCategoria);
            //    entity.Property(c => c.IdCategoria).IsRequired().ValueGeneratedOnAdd();
            //});

            //modelBuilder.Entity<CAT_ENCAPSULATED_PARTITION_TYPE>(entity =>
            //{
            //    entity.HasKey(p => p.IdProducto);
            //    entity.Property(p => p.IdProducto).IsRequired().ValueGeneratedOnAdd();
            //    entity.HasOne(p => p.RefCategoria).WithMany(c => c.Productos)
            //    .HasForeignKey(p => p.IdCategoria);
            //})
        }

        private List<string> GetDatabaseFileList()
        {
            List<string> fileList = new List<string>()
            {
                ApplicationConstants.SCRIPT_DATABASE_FILE_V1,
            };

            return fileList;
        }


        //private bool ValidateTableControl<T>(string columnName) where T : class
        //{
        //    bool isUpdated = false;

        //    T entity = this.Set<T>().FirstOrDefault();
        //    if (entity != null)
        //    {
        //        DateTime entityDateTime = (DateTime)entity
        //            .GetType().GetProperties()
        //            .Where(a => a.Name == columnName).Select(p => p.GetValue(entity, null))
        //            .FirstOrDefault();

        //        isUpdated = DateTime.Now.Subtract(entityDateTime).TotalHours <= 24;
        //    }

        //    return isUpdated;
        //}

        private bool ValidateTableControl<T>() where T : class
        {
            bool isUpdated = false;

            string tableName = typeof(T).Name.ToUpper();
            SEC_TABLE_CONTROL entity = this.Set<SEC_TABLE_CONTROL>().FirstOrDefault(f => f.Table_Name.ToUpper() == tableName);
            if (entity != null)
            {
                isUpdated = DateTime.Now.Subtract(entity.Last_Update).TotalMinutes <= minutesRefreshData;
            }

            if (isUpdated)
            {
                isUpdated = this.Set<T>().Count() > 0;
            }

            return isUpdated;
        }

        public void LoadTable<T>(Func<DbSet<T>, bool> functionUpdate) where T : class
        {
            string tableName = typeof(T).Name.ToUpper();
            if (!ValidateTableControl<T>())
            {
                if (this.Set<T>().Count() > 0)
                {
                    this.Set<T>().RemoveRange(this.Set<T>());
                    this.SaveChanges();
                }

                if (functionUpdate(this.Set<T>()))
                {
                    SEC_TABLE_CONTROL controlRegedit = SEC_TABLE_CONTROL.FirstOrDefault(f => f.Table_Name.ToUpper() == tableName);
                    controlRegedit.Last_Update = DateTime.Now;

                    this.Entry(controlRegedit).State = EntityState.Modified;
                    this.SaveChanges();
                }
            }
        }

        public async Task ExecuteScriptDatabase()
        {
            string sqlFolder = Utilities.GetApplicationPath();
            List<string> scriptFileList = GetDatabaseFileList();

            if (scriptFileList.Count > 0)
            {
                foreach (string sqlFileName in scriptFileList)
                {
                    string fileContent = await Utilities.GetRawFile(sqlFileName);
                    if (fileContent != null)
                    {
                        try
                        {
                            Database.ExecuteSqlRaw(fileContent);
                        }
                        catch(Exception) { }
                    }
                }
            }
        }

        public bool FileUpdateOrCreateRequired(string fileName, string fileType, float version = 1)
        {
            bool isRequired = false;

            CAT_FILE catFile = CAT_FILE.FirstOrDefault(f => f.File_Name == fileName && f.File_Type == fileType && f.Version == version);
            if (catFile == null || DateTime.Now.Subtract(catFile.Modification_Date).TotalMinutes > minutesRefreshData)
            {
                isRequired = true;
            }

            return isRequired;
        }

        public void CreateFile(string fileName, string fileType, string source, float version = 1)
        {
            
            CAT_FILE catFile = CAT_FILE.FirstOrDefault(f => f.File_Name == fileName && f.File_Type == fileType && f.Version == version);

            if (catFile == null)
            {
                CAT_FILE newFile = new CAT_FILE
                {
                    File_Name = fileName,
                    File_Type = fileType,
                    Source = source,
                    Version = version,
                    Creation_Date = DateTime.Now,
                    Modification_Date = DateTime.Now,
                    Active = true
                };

                CAT_FILE.Add(newFile);
                SaveChanges();
            }
            else
            {
                if (DateTime.Now.Subtract(catFile.Modification_Date).TotalMinutes > minutesRefreshData)
                {
                    catFile.Source = source;
                    catFile.Version = version;
                    catFile.Creation_Date = DateTime.Now;
                    catFile.Modification_Date = DateTime.Now;
                    Entry(catFile).State = EntityState.Modified;
                    SaveChanges();
                }
            }
        }
    }
}
