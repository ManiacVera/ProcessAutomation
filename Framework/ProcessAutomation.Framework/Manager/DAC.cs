using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using ProcessAutomation.Framework.DTO.Enum;
using Microsoft.Data.Sqlite;
using ProcessAutomation.Framework.DTO.Contract;
using static ProcessAutomation.Framework.Shared.EncryptionShared;

namespace ProcessAutomation.Framework.Manager
{
    public class DAC : IDAC
    {
        private string stringConnection = null;
        private static Hashtable stringConnectionTable = new Hashtable();
        private string lastError = null;
        private bool hasError = false;
        private DbConnection manualConnection = null;

        private struct ConnectionStructure<T>
        {
            public DbConnection connection;
            public DbCommand command;
            public string query;
            public CommandType commandType;
            public T result;
        }

        public DatabaseTypeEnum dbType;

        //public DAC(LoggerManager loggerManager)
        public DAC()
        {
            //this.loggerManager = loggerManager;
            if (stringConnectionTable.Count == 0)
            {
                stringConnectionTable.Add(DatabaseTypeEnum.SQLSERVER, "Data Source = {0},{1}; initial Catalog = {2}; user Id = {3}; Password = {4}; Connection Timeout = {5}; Encrypt={6}; TrustServerCertificate={7}");
                stringConnectionTable.Add(DatabaseTypeEnum.MYSQL, "Server={0};Port={1};Database={2};Uid={3};Pwd={4};");
                stringConnectionTable.Add(DatabaseTypeEnum.ORACLE, "Data Source = {0}; initial Catalog = {1},{2}; user Id = {3}; Password = {4}; Connection Timeout = {5}");
                stringConnectionTable.Add(DatabaseTypeEnum.POSTGRESS, "Data Source = {0}; initial Catalog = {1},{2}; user Id = {3}; Password = {4}; Connection Timeout = {5}");
                stringConnectionTable.Add(DatabaseTypeEnum.ACCESS, "Data Source = {0}; initial Catalog = {1},{2}; user Id = {3}; Password = {4}; Connection Timeout = {5}");
            }
        }

        private int GetDefaultPort(DatabaseTypeEnum databaseType)
        {
            int port = 0;

            switch (databaseType)
            {
                case DatabaseTypeEnum.SQLSERVER:
                    port = 1433;
                    break;
                case DatabaseTypeEnum.MYSQL:
                    port = 3306;
                    break;
                case DatabaseTypeEnum.SQLITE:
                    port = 0;
                    break;
                default:
                    port = 1433;
                    break;
            }
            return port;
        }

        private string ParameterProcess(object[] parameters)
        {
            string values = string.Empty;
            if (parameters != null && parameters.Length > 0)
            {
                foreach (object parameter in parameters)
                {
                    if (parameter == null)
                        values += "null,";
                    else if (parameter.GetType().ToString() == typeof(string).ToString()
                             || parameter.GetType().ToString() == typeof(char).ToString())
                    {
                        values += "'" + parameter + "',";
                    }
                    else if (parameter.GetType().ToString() == typeof(DBNull).ToString())
                        values += "null,";
                    else
                        values += parameter.ToString() + ",";

                }

                values = values.Substring(0, values.Length - 1);
            }

            return values;
        }

        private string ProcessQuery(string query, object[] values, object[] filter)
        {
            if (!string.IsNullOrEmpty(query))
            {
                if (!string.IsNullOrEmpty(query))
                {
                    if (values != null && values.Length > 0 && filter != null && filter.Length > 0)
                    {
                        query = string.Format(query, values, filter);
                    }
                    else if (values != null && values.Length > 0)
                    {
                        query = string.Format(query, values);
                    }
                    if (filter != null && filter.Length > 0)
                    {
                        query = string.Format(query, filter);
                    }
                }
            }

            return query;
        }

        private DbConnection GetDbConnection()
        {
            DbConnection connection = null;
            string stringConnectionDecrypted = Encryption.Decrypt(stringConnection);

            switch (dbType)
            {
                case DatabaseTypeEnum.SQLSERVER:
                    connection = new SqlConnection(stringConnectionDecrypted);
                    break;
                case DatabaseTypeEnum.SQLITE:
                    connection = new SqliteConnection(stringConnectionDecrypted);
                    break;
            }

            return connection;
        }

        private DbCommand GetDbCommand()
        {
            DbCommand command = null;

            switch (dbType)
            {
                case DatabaseTypeEnum.SQLSERVER:
                    command = new SqlCommand();
                    break;
                case DatabaseTypeEnum.SQLITE:
                    command = new SqliteCommand();
                    break;
            }

            return command;
        }

        private DbDataAdapter GetDbAdapter(DbCommand command)
        {
            DbDataAdapter adapter = null;

            switch (dbType)
            {
                case DatabaseTypeEnum.SQLSERVER:
                    adapter = new SqlDataAdapter((SqlCommand)command);
                    break;
                case DatabaseTypeEnum.SQLITE:
                    adapter = new SqliteDataAdapter((SqliteCommand)command);
                    break;
            }

            return adapter;
        }

        private T ExecuteConnection<T>(string query, CommandType commandType, Func<ConnectionStructure<T>, T> commandMethod)
        {
            ConnectionStructure<T> connectionStructure = new ConnectionStructure<T>();
            connectionStructure.query = query;
            connectionStructure.commandType = commandType;
            //DataSet dataSet = new DataSet();

            try
            {
                if (manualConnection != null)
                {
                    if (manualConnection.State != ConnectionState.Open)
                    {
                        if (manualConnection.State != ConnectionState.Closed)
                        {
                            try
                            {
                                manualConnection.Close();
                                manualConnection.Dispose();
                                manualConnection = null;
                            }
                            catch (Exception)
                            { }
                        }

                        OpenConnection();
                    }

                    connectionStructure.connection = manualConnection;
                }
                else
                {
                    connectionStructure.connection = GetDbConnection();
                    connectionStructure.connection.Open();
                }
                
                connectionStructure.command = GetDbCommand();
                connectionStructure.command.CommandType = commandType;
                connectionStructure.command.CommandText = query;
                connectionStructure.command.CommandTimeout = connectionStructure.connection.ConnectionTimeout;
                connectionStructure.command.Connection = connectionStructure.connection;
                connectionStructure.result = commandMethod(connectionStructure);
            }
            catch (Exception)
            {
                //loggerManager.WriteLog(ex.Message, LoggerManager.eMessageType.ERROR, true);
                throw;
            }
            finally
            {
                if (manualConnection == null && connectionStructure.connection?.State == ConnectionState.Open)
                {
                    connectionStructure.connection.Close();
                    connectionStructure.command.Connection.Close();
                    connectionStructure.command.Dispose();
                    //connectionStructure.connection.Dispose();
                }
            }

            return connectionStructure.result;
        }

        public DatabaseTypeEnum GetDbType()
        {
            return dbType;
        }

        public void SetConnection(string stringConnection, DatabaseTypeEnum dbType = DatabaseTypeEnum.SQLSERVER)
        {
            hasError = false;
            if (!string.IsNullOrEmpty(stringConnection))
            {
                this.dbType = dbType;
                this.stringConnection = Encryption.Encrypt(stringConnection);

                //if (connection != null && connection.ConnectionString != connectionString)
                //{
                //    try
                //    {
                //        connection.Close();
                //        connection.Dispose();
                //    }
                //    catch (SqlException ex)
                //    {
                //        hasError = true;
                //        lastError = string.Format(SQL_EXCEPTION_FORMAT, ex.Number, ex.ErrorCode, ex.Message, ex.StackTrace);
                //    }
                //    catch (Exception ex)
                //    {
                //        hasError = true;
                //        lastError = string.Format(EXCEPTION_FORMAT, ex.Message, ex.StackTrace);
                //    }
                //}

                //try
                //{
                //    connection = new SqlConnection(stringConnection);
                //}
                //catch (SqlException ex)
                //{
                //    hasError = true;
                //    lastError = string.Format(SQL_EXCEPTION_FORMAT, ex.Number, ex.ErrorCode, ex.Message, ex.StackTrace);
                //}
                //catch (Exception ex)
                //{
                //    hasError = true;
                //    lastError = string.Format(EXCEPTION_FORMAT, ex.Message, ex.StackTrace);
                //}
            }
        }

        public void SetConnection(DatabaseTypeEnum dbType, string server, string database, int port, string user, string password, int timeOut = 0)
        {
            this.dbType = dbType;
            stringConnection = string.Format(stringConnectionTable[dbType].ToString(), server, port, database, user, password, timeOut);
            stringConnection = Encryption.Encrypt(stringConnection);
        }

        public DataTable ExecuteStoredProcedureToDataTable(string query, Dictionary<string, object> parameterArray)
        {
            DataSet dataSet = ExecuteStoredProcedureToDataSet(query, parameterArray);
            return (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null);
        }

        public DataSet ExecuteStoredProcedureToDataSet(string query, Dictionary<string, object> parameterArray)
        {
            return ExecuteConnection(query, CommandType.StoredProcedure, (ConnectionStructure<DataSet> connectionStructure) =>
            {
                connectionStructure.result = new DataSet();
                DbDataAdapter adapter = GetDbAdapter(connectionStructure.command);

                if (parameterArray != null && parameterArray.Count() > 0)
                {
                    foreach (KeyValuePair<string, object> keyValue in parameterArray)
                    {
                        DbParameter dbParameter = connectionStructure.command.CreateParameter();
                        dbParameter.ParameterName = string.Format("@{0}", keyValue.Key);
                        dbParameter.Value = keyValue.Value;
                        connectionStructure.command.Parameters.Add(dbParameter);
                    }
                }

                adapter.Fill(connectionStructure.result);
                adapter.Dispose();
                return connectionStructure.result;
            });
        }

        public List<T> ExecuteQueryToList<T>(string query)
        {
            DataSet dataSet = ExecuteQueryToDataSet(query);
            return (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0 ? DataManager.DataTableToList<T>(dataSet.Tables[0]) : null);
        }

        public DataTable ExecuteQueryToDataTable(string query)
        {
            DataSet dataSet = ExecuteQueryToDataSet(query);
            return (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null);
        }

        public DataSet ExecuteQueryToDataSet(string query)
        {
            return ExecuteConnection(query, CommandType.Text, (ConnectionStructure<DataSet> connectionStructure) =>
            {
                connectionStructure.result = new DataSet();

                switch (dbType)
                {
                    case DatabaseTypeEnum.ACCESS:
                        {
                        }
                        break;
                    default:
                        {
                            DbDataAdapter adapter = GetDbAdapter(connectionStructure.command);
                            adapter.Fill(connectionStructure.result);
                            adapter.Dispose();
                        }
                        break;
                }
                
                return connectionStructure.result;
            });
        }

        public void OpenConnection()
        {
            CloseConnection();

            manualConnection = GetDbConnection();

            try
            {
                manualConnection.Open();
            }
            catch (Exception)
            { }
        }

        public void CloseConnection()
        {
            if (manualConnection?.State == ConnectionState.Open)
            {
                try
                {
                    manualConnection.Close();
                    manualConnection.Dispose();
                }
                catch (Exception)
                { }

                //manualConnection.Dispose();
                manualConnection = null;
            }
        }

        public int ExecuteNonQuery(string query)
        {
            return ExecuteConnection(query, CommandType.Text, (ConnectionStructure<int> connectionStructure) => connectionStructure.command.ExecuteNonQuery());
        }

        public object ExecuteScalar(string query)
        {
            return ExecuteConnection(query, CommandType.Text, (ConnectionStructure<object> connectionStructure) => connectionStructure.command.ExecuteScalar());
        }

        //public bool CreateLocalDatabase(string database)
        //{
        //    bool created = false;
        //    //const string createDatabaseFormat = "CREATE DATABASE {0}";

        //    if (!string.IsNullOrWhiteSpace(database))
        //    {
        //        try
        //        {
        //            switch (dbType)
        //            {
        //                case DatabaseTypeEnum.SQLSERVER:
        //                    {

        //                    }
        //                    break;
        //                case DatabaseTypeEnum.SQLITE:
        //                    //SQLiteConnection.CreateFile(database);
        //                    break;
        //            }

        //            created = true;
        //        }
        //        catch (Exception ex)
        //        { }
        //    }

        //    return created;
        //}

        //public bool CreateTable(string table, string columns, DatabaseTypeEnum dbType)
        //{
        //    bool created = false;

        //    try
        //    {
        //        string tableFormat = null;

        //        switch (dbType)
        //        {
        //            case DatabaseTypeEnum.SQLSERVER:
        //                break;
        //            default:
        //                break;
        //        }

        //        string[] columnList = columns.Split(',');

        //        if (columnList != null && columnList.Length > 0)
        //        {
        //            foreach (string column in columnList)
        //            { 

        //            }
        //        }

        //        created = true;
        //    }
        //    catch (Exception ex)
        //    { }

        //    return created;
        //}

        public Dictionary<string, object> ExecuteStoredProcedure(string query, int counter, Dictionary<string, object> parameterArray)
        {
            Dictionary<string, object> resultList = null;

            if (!string.IsNullOrEmpty(query))
            {
                DataSet dataSet = ExecuteStoredProcedureToDataSet(query, parameterArray);

                if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
                {
                    if (resultList == null)
                    {
                        resultList = new Dictionary<string, object>();
                    }

                    int tableCounter = 0;
                    foreach (DataTable dataTable in dataSet.Tables)
                    {
                        resultList.Add(tableCounter.ToString(), DataManager.DataTableToDictionaryList(dataTable));
                        tableCounter++;
                    }
                }
            }

            return resultList;
        }

        public Dictionary<string, object> ExecuteStoredProcedure(string query, int counter, object[] filter)
        {
            Dictionary<string, object> resultList = null;

            //if (!string.IsNullOrEmpty(query))
            //{
            //    DataSet dataSet = ExecuteStoredProcedureToDataSet(query, parameterArray);

            //    if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
            //    {
            //        if (resultList == null)
            //        {
            //            resultList = new Dictionary<string, object>();
            //        }

            //        int tableCounter = 0;
            //        foreach (DataTable dataTable in dataSet.Tables)
            //        {
            //            resultList.Add(tableCounter.ToString(), DataManager.DataTableToDictionaryList(dataTable));
            //            tableCounter++;
            //        }
            //    }
            //}

            return resultList;
        }

        public List<T> ExecuteStoredProcedure<T>(string query, int counter, Dictionary<string, object> parameterArray)
        {
            List<T> resultList = null;

            if (!string.IsNullOrEmpty(query))
            {
                resultList = DataManager.DataTableToList<T>(ExecuteStoredProcedureToDataTable(query, parameterArray));
                if (counter > 0)
                {
                    resultList = resultList.Take(counter).ToList();
                }
            }

            return resultList;
        }

        public List<T> ExecuteQueryToList<T>(string query, int counter, object[] filter)
        {
            List<T> resultList = null;

            string queryProcessed = ProcessQuery(query, null, filter);
            if (!string.IsNullOrEmpty(queryProcessed))
            {
                resultList = ExecuteQueryToList<T>(queryProcessed);
                if (counter > 0)
                {
                    resultList = resultList.Take(counter).ToList();
                }
            }

            return resultList;
        }

        public List<Dictionary<string, object>> RetrieveListByQuery(string query, int counter, object[] filter)
        {
            List<Dictionary<string, object>> resultList = null;

            string queryProcessed = ProcessQuery(query, null, filter);
            if (!string.IsNullOrEmpty(queryProcessed))
            {
                resultList = DataManager.DataTableToDictionaryList(ExecuteQueryToDataTable(queryProcessed));
                if (counter > 0)
                {
                    resultList = resultList.Take(counter).ToList();
                }
            }

            return resultList;
        }

        public int CreateByQuery(string query, object[] values)
        {
            int result = 0;

            string queryProcessed = ProcessQuery(query, values, null);
            if (!string.IsNullOrEmpty(queryProcessed))
            {
                result = ExecuteNonQuery(queryProcessed);
            }

            return result;
        }

        public Dictionary<string, object> RetrieveByQuery(string query, int counter, object[] filter)
        {
            Dictionary<string, object> result = null;
            List<Dictionary<string, object>> resultList = RetrieveListByQuery(query, counter, filter);

            if (resultList != null && resultList.Count > 0)
            {
                result = resultList.FirstOrDefault();
            }

            return result;
        }

        public int UpdateByQuery(string query, object[] values, object[] filter = null)
        {
            int result = 0;

            string queryProcessed = ProcessQuery(query, values, filter);
            if (!string.IsNullOrEmpty(queryProcessed))
            {
                result = ExecuteNonQuery(queryProcessed);
            }

            return result;
        }

        public int DeleteByQuery(string query, object[] filter)
        {
            int result = 0;

            string queryProcessed = ProcessQuery(query, null, filter);
            if (!string.IsNullOrEmpty(queryProcessed))
            {
                result = ExecuteNonQuery(queryProcessed);
            }

            return result;
        }

        public List<Dictionary<string, object>> RetrieveInfoByQuery(string query, int counter, Dictionary<string, object> parameterArray)
        {
            List<Dictionary<string, object>> resultList = null;

            if (!string.IsNullOrEmpty(query))
            {
                resultList = DataManager.DataTableToDictionaryList(ExecuteStoredProcedureToDataTable(query, parameterArray));
                if (counter > 0)
                {
                    resultList = resultList.Take(counter).ToList();
                }
            }

            return resultList;
        }

        public bool HasError()
        {
            return hasError;
        }

        public string GetLastError()
        {
            return lastError;
        }

        //public DataTable ExecuteNonQuery(string query)
        //{
        //    hasError = false;
        //    lastError = null;

        //    DataTable result = new DataTable();
        //    if (connection != null)
        //    {
        //        try
        //        {
        //            SqlCommand command = new SqlCommand();
        //            command.CommandType = System.Data.CommandType.Text;
        //            command.CommandText = query;
        //            command.Connection = connection;
        //            connection.Open();

        //            SqlDataAdapter adapter = new SqlDataAdapter(command);
        //            adapter.Fill(result);
        //            adapter.Dispose();
        //        }
        //        catch (SqlException ex)
        //        {
        //            hasError = true;
        //            lastError = string.Format(SQL_EXCEPTION_FORMAT, ex.Number, ex.ErrorCode, ex.Message, ex.StackTrace);
        //        }
        //        catch (Exception ex)
        //        {
        //            hasError = true;
        //            lastError = string.Format(EXCEPTION_FORMAT, ex.Message, ex.StackTrace);
        //        }
        //        finally
        //        {
        //            connection.Close();
        //        }
        //    }

        //    return result;
        //}
    }
}


