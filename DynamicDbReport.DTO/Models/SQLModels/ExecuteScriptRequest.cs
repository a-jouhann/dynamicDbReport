namespace DynamicDbReport.DTO.Models.SQLModels;

public class ExecuteScriptRequest
{
    public CredentialRequest Credential { get; set; }

    public string QueryToExecute { get; set; }
    public bool NoCount { get; set; }

}
