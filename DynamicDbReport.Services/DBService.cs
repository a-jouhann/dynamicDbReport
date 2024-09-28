using DynamicDbReport.DTO.Models.Public;
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
    
    public DatabaseNameListResponse DBNameList(CredentialRequest requestModel)
    {
        try
        {
            var instance = DBFactory.CreateInstance(requestModel.Engine);
            return instance.DBNameList(requestModel);
        }
        catch (Exception x)
        {
            return new() { ErrorException = new() { ErrorMessage = $"{x.Message} - {x.InnerException?.Message}" } };
        }
    }

    
    public ExecuteScriptResponse ExecuteScript(ExecuteScriptRequest requestModel)
    {
        try
        {
            var instance = DBFactory.CreateInstance(requestModel.Credential.Engine);
            return instance.ExecuteScript(requestModel);
        }
        catch (Exception x)
        {
            return new() { ErrorException = new() { ErrorMessage = $"{x.Message} - {x.InnerException?.Message}" } };
        }
    }


    public async Task<PublicActionResponse> ImportTable(ImportTableRequest requestModel)
    {

        try
        {
            var instance = DBFactory.CreateInstance(requestModel.Credential.Engine);
            return await instance.ImportTable(requestModel);
        }
        catch (Exception x)
        {
            return new() { ErrorException = new() { ErrorMessage = $"{x.Message} - {x.InnerException?.Message}" } };
        }
    }



}
