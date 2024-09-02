using DynamicDbReport.DTO.Models.SQLModels;
using Microsoft.Data.SqlClient;

namespace DynamicDbReport.Services.Providers;

internal class DB_MSSQL : IPublicDBFunctions
{


    public CheckCredentialResponse CheckDBConnection(CredentialRequest credential)
    {
        string connectionString = $"Server={credential.ServerAddress};User Id={credential.Username};Password={credential.Password};TrustServerCertificate=True;";

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


}
