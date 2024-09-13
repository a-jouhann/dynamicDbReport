using DynamicDbReport.DTO.Models.SQLModels;
using DynamicDbReport.Services.DBContext;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace DynamicDbReport.Services.Providers;

internal class DB_POSTGRESQL : IPublicDBFunctions
{

    private string CreateConnectionString(CredentialRequest requestModel) => $"Server={requestModel.ServerAddress};Port={requestModel.DBPort};{(string.IsNullOrEmpty(requestModel.DbName) || requestModel.DbName == "" ? "" : $"Database={requestModel.DbName};")}User Id={requestModel.Username};Password={requestModel.Password};";

    public CheckCredentialResponse CheckDBConnection(CredentialRequest credential)
    {
        credential.DbName = string.Empty;
        string connectionString = CreateConnectionString(credential);

        try
        {
            using NpgsqlConnection connection = new(connectionString);
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
            using NpgsqlConnection connection = new(connectionString);
            connection.Open();

            List<string> dbs = [];
            using (NpgsqlCommand command = new("SELECT datname FROM pg_database WHERE datistemplate = false", connection))
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                    dbs.Add(reader.GetString(0));
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
