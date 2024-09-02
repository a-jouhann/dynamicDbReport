using DynamicDbReport.DTO.Models.SQLModels;
using DynamicDbReport.Services.Providers;

namespace DynamicDbReport.Services;

public class DBService
{

    public CheckCredentialResponse CheckDBConnection(CredentialRequest credential)
    {
        try
        {
            var instance = DBFactory.CreateInstance(credential.Engine);
            return instance.CheckDBConnection(credential);
        }
        catch (Exception x)
        {
            return new() { ErrorException = new() { ErrorMessage = $"{x.Message} - {x.InnerException?.Message}" } };
        }
    }





}
