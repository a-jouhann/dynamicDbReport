using DynamicDbReport.DTO.Models.Public;

namespace DynamicDbReport.Services.Providers;

internal class DBFactory
{

    internal static IPublicDBFunctions CreateInstance(EngineName engineName) =>
        engineName switch
        {
            EngineName.MSSQL => new DB_MSSQL(),


            _ => throw new NotImplementedException()
        };

}
