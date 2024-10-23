namespace DynamicDbReport.DTO.Models.SQLModels;

public class RowItemDetails
{
    public short ColumnIndex { get; set; }
    public string ItemValue { get; set; }
    public bool NullItem { get; set; } = false;
}
