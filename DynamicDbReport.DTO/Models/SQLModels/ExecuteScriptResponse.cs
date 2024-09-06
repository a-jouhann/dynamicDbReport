using DynamicDbReport.DTO.Models.Public;

namespace DynamicDbReport.DTO.Models.SQLModels;

public class ExecuteScriptResponse
{
    public string ResponesMessage { get; set; }
    public List<string> ColumnName { get; set; }
    public List<List<string>> Rows { get; set; }

    public ErrorExceptionResponse ErrorException { get; set; }
}
