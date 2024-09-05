using DynamicDbReport.DTO.Models.SQLModels;
using Microsoft.Data.SqlClient;

namespace DynamicDbReport.Services.Providers;

internal class DB_MSSQL : IPublicDBFunctions
{


    private string CreateConnectionString(CredentialRequest requestModel) => $"Server={requestModel.ServerAddress};User Id={requestModel.Username};Password={requestModel.Password};TrustServerCertificate=True;";

    public CheckCredentialResponse CheckDBConnection(CredentialRequest credential)
    {
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
        string connectionString = CreateConnectionString(requestModel);        
        try
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            List<string> dbs = [];
            using (SqlCommand command = new SqlCommand("SELECT name FROM sys.databases", connection))
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


}
