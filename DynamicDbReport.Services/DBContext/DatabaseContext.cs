using DynamicDbReport.DTO.Models.DBContext;
using DynamicDbReport.DTO.Models.Public;
using Microsoft.EntityFrameworkCore;

namespace DynamicDbReport.Services.DBContext;

public class DatabaseContext : DbContext
{
    private readonly DBConnectionInject _connection;
    public DatabaseContext(DBConnectionInject connection)
    {
        _connection = connection;
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
       : base(options)
    {
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_connection is null || (byte)_connection.Engine < 0 || string.IsNullOrEmpty(_connection.ConnectionString) || _connection.ConnectionString == "")
            throw new Exception("Send Connection object");

        if (optionsBuilder.IsConfigured) return;

        switch (_connection.Engine)
        {
            case EngineName.MSSQL:
                optionsBuilder.UseSqlServer(_connection.ConnectionString);
                break;


            default:
                throw new NotImplementedException();

        }
    }

}
