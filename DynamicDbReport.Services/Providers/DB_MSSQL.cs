using DynamicDbReport.DTO.Models.SQLModels;
using DynamicDbReport.Services.DBContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DynamicDbReport.Services.Providers;

internal class DB_MSSQL : IPublicDBFunctions
{

    private string CreateConnectionString(CredentialRequest requestModel) => $"Server={requestModel.ServerAddress}{(requestModel.DBPort == "1433" ? "" : $", {requestModel.DBPort}")};{(string.IsNullOrEmpty(requestModel.DbName) || requestModel.DbName == "" ? "" : $"Database={requestModel.DbName};")}User Id={requestModel.Username};Password={requestModel.Password};TrustServerCertificate=True;";

    public CheckCredentialResponse CheckDBConnection(CredentialRequest credential)
    {
        credential.DbName = string.Empty;
        string connectionString = CreateConnectionString(credential);

        try
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();
            return new() { SuccessAction = true, ResponseData = true };
        }
        catch (Exception e)
        {
            return new() { ErrorException = new() { ErrorMessage = e.Message } };
        }
    }

    public DatabaseNameListResponse DBNameList(CredentialRequest requestModel)
    {
        requestModel.DbName = string.Empty;
        string connectionString = CreateConnectionString(requestModel);
        try
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            List<string> dbs = [];
            using (SqlCommand command = new("SELECT name FROM sys.databases", connection))
            {
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    dbs.Add(reader["name"].ToString());
            }
            return new() { SuccessAction = true, ResponseData = dbs };
        }
        catch (Exception e)
        {
            return new() { ErrorException = new() { ErrorMessage = e.Message } };
        }
    }

    private ExecuteScriptResponse ExecuteDynamicQuery(DatabaseContext db, string sqlQuery)
    {
        try
        {
            var connection = db.Database.GetDbConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sqlQuery;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            using var result = command.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(result);
            ExecuteScriptResponse responseObject = new() { ResponseData = new() { Columns = [], Rows = [] }, SuccessAction = true };

            foreach (DataColumn column in dataTable.Columns)
                responseObject.ResponseData.Columns.Add(new() { ColumnName = column.ColumnName, ColumnType = column.DataType.Name, Length = column.MaxLength });


            foreach (DataRow row in dataTable.Rows)
            {
                List<string> currentItems = [];
                foreach (DataColumn column in dataTable.Columns)
                    currentItems.Add(row[column].ToString());

                responseObject.ResponseData.Rows.Add(currentItems);
            }

            //message
            return responseObject;
        }
        catch (Exception x)
        {
            return new() { ResponseData = new() { ResponesMessage = x.Message }, ErrorException = new() { ErrorMessage = x.Message } };
        }
    }

    public ExecuteScriptResponse ExecuteScript(ExecuteScriptRequest requestModel)
    {
        using DatabaseContext db = new(new DTO.Models.DBContext.DBConnectionInject() { ConnectionString = CreateConnectionString(requestModel.Credential), Engine = requestModel.Credential.Engine });
        var responseObject = ExecuteDynamicQuery(db, requestModel.QueryToExecute);
        if (responseObject?.ResponseData?.Columns is not null && responseObject.ResponseData.Columns.Count > 0)
            responseObject.ResponseData.ResponesMessage += $"{(requestModel.NoCount ? "" : $"({responseObject.ResponseData.Rows.Count} rows affected)")} \r\n Completion time: {DateTime.Now:yyyy-MM-ddTHH:mm:ss.ffff}";

        return responseObject;
    }

}
