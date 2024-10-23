namespace DynamicDbReport.DTO.Models.SQLModels;

public class ImportTableRequest : CreateTableRequest
{
    public List<List<RowItemDetails>> Rows { get; set; }
}
