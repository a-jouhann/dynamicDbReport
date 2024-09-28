using DynamicDbReport.DTO.Models.Public;
using DynamicDbReport.DTO.Models.SQLModels;

namespace DynamicDbReport.Services.Providers;

interface IPublicDBFunctions
{

    CheckCredentialResponse CheckDBConnection(CredentialRequest credential);
    DatabaseNameListResponse DBNameList(CredentialRequest requestModel);
    ExecuteScriptResponse ExecuteScript(ExecuteScriptRequest requestModel);
    Task<PublicActionResponse> ImportTable(ImportTableRequest requestModel);




}
