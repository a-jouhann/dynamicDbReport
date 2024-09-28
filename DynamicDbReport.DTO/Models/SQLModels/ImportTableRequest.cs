using System.Data;

namespace DynamicDbReport.DTO.Models.SQLModels;

public class ImportTableRequest
{
    public CredentialRequest Credential { get; set; }

    public string TableName { get; set; }
    public DataTable dataTable { get; set; }
}
