using DynamicDbReport.DTO.Models.SQLModels;

namespace DynamicDbReport.Services.Providers;

interface IPublicDBFunctions
{

    CheckCredentialResponse CheckDBConnection(CredentialRequest credential);






}
