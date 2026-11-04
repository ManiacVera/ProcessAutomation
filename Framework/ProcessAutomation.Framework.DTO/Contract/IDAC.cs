using ProcessAutomation.Framework.DTO.Enum;
using System.Collections.Generic;
using System.Data;

namespace ProcessAutomation.Framework.DTO.Contract
{
    public interface IDAC
    {
        DatabaseTypeEnum GetDbType();
        void SetConnection(string stringConnection, DatabaseTypeEnum dbType = DatabaseTypeEnum.SQLSERVER);
        void SetConnection(DatabaseTypeEnum dbType, string server, string database, int port, string user, string password, int timeOut = 0);
        DataTable ExecuteStoredProcedureToDataTable(string query, Dictionary<string, object> parameterArray);
        DataSet ExecuteStoredProcedureToDataSet(string query, Dictionary<string, object> parameterArray);
        List<T> ExecuteQueryToList<T>(string query);
        DataTable ExecuteQueryToDataTable(string query);
        DataSet ExecuteQueryToDataSet(string query);
        void OpenConnection();
        void CloseConnection();
        int ExecuteNonQuery(string query);
        object ExecuteScalar(string query);
        Dictionary<string, object> ExecuteStoredProcedure(string query, int counter, Dictionary<string, object> parameterArray);
        Dictionary<string, object> ExecuteStoredProcedure(string query, int counter, object[] filter);
        List<T> ExecuteStoredProcedure<T>(string query, int counter, Dictionary<string, object> parameterArray);
        List<T> ExecuteQueryToList<T>(string query, int counter, object[] filter);
        List<Dictionary<string, object>> RetrieveListByQuery(string query, int counter, object[] filter);
        int CreateByQuery(string query, object[] values);
        Dictionary<string, object> RetrieveByQuery(string query, int counter, object[] filter);
        int UpdateByQuery(string query, object[] values, object[] filter = null);
        int DeleteByQuery(string query, object[] filter);
        List<Dictionary<string, object>> RetrieveInfoByQuery(string query, int counter, Dictionary<string, object> parameterArray);
        bool HasError();
        string GetLastError();
    }
}
