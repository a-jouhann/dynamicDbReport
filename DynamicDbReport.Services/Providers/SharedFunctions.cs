using DynamicDbReport.DTO.Models.SQLModels;
using DynamicDbReport.Services.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DynamicDbReport.Services.Providers;

internal class SharedFunctions
{

    public static ExecuteScriptResponse ExecuteDynamicQuery(DatabaseContext db, string sqlQuery)
    {
        try
        {
            var connection = db.Database.GetDbConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sqlQuery;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            using var result = command.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(result);
            ExecuteScriptResponse responseObject = new() { ResponseData = new() { Columns = [], Rows = [] }, SuccessAction = true };

            foreach (DataColumn column in dataTable.Columns)
                responseObject.ResponseData.Columns.Add(new() { ColumnName = column.ColumnName, ColumnType = column.DataType.Name, Length = column.MaxLength });


            foreach (DataRow row in dataTable.Rows)
            {
                List<string> currentItems = [];
                foreach (DataColumn column in dataTable.Columns)
                    currentItems.Add(row[column].ToString());

                responseObject.ResponseData.Rows.Add(currentItems);
            }

            //message
            return responseObject;
        }
        catch (Exception x)
        {
            return new() { ResponseData = new() { ResponesMessage = x.Message }, ErrorException = new() { ErrorMessage = x.Message } };
        }
    }
}
