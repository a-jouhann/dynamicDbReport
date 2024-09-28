using DynamicDbReport.DTO.Models.Public;

namespace DynamicDbReport.DTO.Models.SQLModels;

public class DatabaseNameListResponse : PublicActionResponse
{
    public List<string> ResponseData { get; set; }
}
