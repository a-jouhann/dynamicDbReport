using DynamicDbReport.DTO.Models.Public;

namespace DynamicDbReport.DTO.Models.SQLModels;

public class ExecuteScriptResponse : PublicActionResponse
{
    public ExecuteDetail ResponseData { get; set; }
    public class ExecuteDetail
    {
        public string ResponesMessage { get; set; }
        public List<ColumnDetail> Columns { get; set; }
        public List<List<string>> Rows { get; set; }
    }

    public class ColumnDetail
    {
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public int Length { get; set; }
    }
}
