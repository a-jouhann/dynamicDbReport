namespace DynamicDbReport.DTO.Models.SQLModels;

public class ColumnDetails
{
    public string ColumnName { get; set; }
    public string ColumnType { get; set; }
    public int Length { get; set; }
    public bool NullableItem { get; set; }
}
