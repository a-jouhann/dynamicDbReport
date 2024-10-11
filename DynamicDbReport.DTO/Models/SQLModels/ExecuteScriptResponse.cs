using DynamicDbReport.DTO.Models.Public;

namespace DynamicDbReport.DTO.Models.SQLModels;

public class ExecuteScriptResponse : PublicActionResponse
{
    public ExecuteDetail ResponseData { get; set; }
    public class ExecuteDetail
    {
        public string ResponesMessage { get; set; }
        public List<ColumnDetails> Columns { get; set; }
        public List<List<RowItemDetails>> Rows { get; set; }
    }

}
