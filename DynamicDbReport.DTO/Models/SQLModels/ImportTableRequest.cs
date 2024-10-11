namespace DynamicDbReport.DTO.Models.SQLModels;

public class ImportTableRequest
{
    public CredentialRequest Credential { get; set; }

    public string TableName { get; set; }
    public List<ColumnDetails> Columns { get; set; }
    public List<List<RowItemDetails>> Rows { get; set; }
}
