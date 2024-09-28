using DynamicDbReport.DTO.Models.Public;

namespace DynamicDbReport.Services.Providers;

internal class DBFactory
{

    internal static IPublicDBFunctions CreateInstance(EngineName engineName) =>
        engineName switch
        {
            EngineName.MSSQL => new DB_MSSQL(),
            EngineName.POSTGRESQL => new DB_POSTGRESQL(),
            EngineName.MYSQL or EngineName.MARIADB => new DB_MYSQL(),

            _ => throw new NotImplementedException()
        };

}
