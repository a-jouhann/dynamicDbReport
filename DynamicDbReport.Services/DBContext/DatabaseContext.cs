using DynamicDbReport.DTO.Models.DBContext;
using DynamicDbReport.DTO.Models.Public;
using Microsoft.EntityFrameworkCore;
using static Org.BouncyCastle.Math.EC.ECCurve;

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

            case EngineName.POSTGRESQL:
                optionsBuilder.UseNpgsql(_connection.ConnectionString, o => o.UseNodaTime());
                break;

            case EngineName.MYSQL:
                optionsBuilder.UseMySQL(_connection.ConnectionString);
                break;

            case EngineName.MARIADB:
                optionsBuilder.UseMySql(_connection.ConnectionString, new MySqlServerVersion(_connection.DBVersion ?? ""));
                break;


            default:
                throw new NotImplementedException();

        }
    }

}
