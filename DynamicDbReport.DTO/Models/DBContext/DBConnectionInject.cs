using DynamicDbReport.DTO.Models.Public;

namespace DynamicDbReport.DTO.Models.DBContext;

public class DBConnectionInject
{
    public string ConnectionString { get; set; }
    public EngineName Engine { get; set; }
}
