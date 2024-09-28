using DynamicDbReport.DTO.Models.Public;
using DynamicDbReport.DTO.Models.SQLModels;
using DynamicDbReport.Services.DBContext;
using MySql.Data.MySqlClient;

namespace DynamicDbReport.Services.Providers;

internal class DB_MYSQL : IPublicDBFunctions
{
    private string CreateConnectionString(CredentialRequest requestModel) => $"server={requestModel.ServerAddress};port={requestModel.DBPort};{(string.IsNullOrEmpty(requestModel.DbName) || requestModel.DbName == "" ? "" : $"database={requestModel.DbName};")}uid={requestModel.Username};pwd={requestModel.Password};";

    public CheckCredentialResponse CheckDBConnection(CredentialRequest credential)
    {
        credential.DbName = string.Empty;
        string connectionString = CreateConnectionString(credential);

        try
        {
            using MySqlConnection connection = new(connectionString);
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
            using MySqlConnection connection = new(connectionString);
            connection.Open();
            MySqlCommand cmd = new("SHOW DATABASES", connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            List<string> DBs = [];
            while (reader.Read())
            {
                DBs.Add(reader[0].ToString());
            }

            return new() { SuccessAction = true, ResponseData = DBs };
        }
        catch (Exception e)
        {
            return new() { ErrorException = new() { ErrorMessage = e.Message } };
        }
    }

    public ExecuteScriptResponse ExecuteScript(ExecuteScriptRequest requestModel)
    {
        using DatabaseContext db = new(new DTO.Models.DBContext.DBConnectionInject() { ConnectionString = CreateConnectionString(requestModel.Credential), Engine = requestModel.Credential.Engine, DBVersion = requestModel.Credential.DBVersion });
        var responseObject = SharedFunctions.ExecuteDynamicQuery(db, requestModel.QueryToExecute);
        if (responseObject?.ResponseData?.Columns is not null && responseObject.ResponseData.Columns.Count > 0)
            responseObject.ResponseData.ResponesMessage += $"{(requestModel.NoCount ? "" : $"({responseObject.ResponseData.Rows.Count} rows affected)")} \r\n Completion time: {DateTime.Now:yyyy-MM-ddTHH:mm:ss.ffff}";

        return responseObject;
    }

    public async Task<PublicActionResponse> ImportTable(ImportTableRequest requestModel)
    {
        string connectionString = CreateConnectionString(requestModel.Credential);
        try
        {
            using (MySqlConnector.MySqlConnection connection = new(connectionString))
            {
                await connection.OpenAsync();
                MySqlConnector.MySqlBulkCopy bulkCopy = new(connection);
                bulkCopy.DestinationTableName = requestModel.TableName;
                await bulkCopy.WriteToServerAsync(requestModel.dataTable);
            }
        }
        catch (Exception x)
        {
            return new() { ErrorException = new() { ErrorMessage = x.Message } };
        }

        return new() { SuccessAction = true };
    }

}
