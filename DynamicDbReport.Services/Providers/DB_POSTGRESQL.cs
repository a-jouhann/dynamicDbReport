using DynamicDbReport.DTO.Models.Public;
using DynamicDbReport.DTO.Models.SQLModels;
using DynamicDbReport.Services.DBContext;
using Npgsql;

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

    public ExecuteScriptResponse ExecuteScript(ExecuteScriptRequest requestModel)
    {
        using DatabaseContext db = new(new DTO.Models.DBContext.DBConnectionInject() { ConnectionString = CreateConnectionString(requestModel.Credential), Engine = requestModel.Credential.Engine });
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
            //using (var connection = new NpgsqlConnection(connectionString))
            //{
            //    await connection.OpenAsync();
            //    using (var bulkCopy = new Npgsql.npgsql(connection))
            //    {
            //        bulkCopy.DestinationTableName = requestModel.TableName;
            //        await bulkCopy.WriteToServerAsync(requestModel.dataTable);
            //    }
            //}
            return new() { ErrorException = new() { ErrorMessage = "not implemented" } };
        }
        catch (Exception x)
        {
            return new() { ErrorException = new() { ErrorMessage = x.Message } };
        }

        return new() { SuccessAction = true };
    }





}
