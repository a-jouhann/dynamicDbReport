namespace DynamicDbReport.DTO.Models.SQLModels;

public class ColumnDetails
{
    public bool Identity { get; set; } = false;
    public string ColumnName { get; set; } = "";
    public string ColumnType { get; set; } = "";
    public int Length { get; set; } = 0;
    public byte Scale { get; set; } = 0;
    public bool NullableItem { get; set; } = true;
    public Guid ID { get; set; } = new();
}
