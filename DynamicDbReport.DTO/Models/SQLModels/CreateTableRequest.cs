namespace DynamicDbReport.DTO.Models.SQLModels;

public class CreateTableRequest
{
    public CredentialRequest Credential { get; set; }
    public bool ReCreateTable { get; set; }
    public string TableName { get; set; }
    public List<ColumnDetails> Columns { get; set; }
}
