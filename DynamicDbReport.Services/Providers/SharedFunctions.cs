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
                responseObject.ResponseData.Columns.Add(new() { ColumnName = column.ColumnName, ColumnType = column.DataType.Name, Length = column.MaxLength, NullableItem = column.AllowDBNull });


            foreach (DataRow row in dataTable.Rows)
            {
                List<RowItemDetails> currentItems = [];
                for (short i = 0; i < dataTable.Columns.Count; i++)
                    currentItems.Add(new() { ItemValue = row[dataTable.Columns[i]].ToString(), ColumnIndex = i, NullItem = dataTable.Columns[i].AllowDBNull && row[dataTable.Columns[i]] is null });

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

    public static DataTable ConvertToDataTable(List<ColumnDetails> columns, List<List<RowItemDetails>> rows)
    {
        DataTable dt = new();
        foreach (var j in columns)
            dt.Columns.Add(j.ColumnName);

        foreach (var j in rows)
        {
            var newRow = dt.NewRow();
            for (int i = 0; i < j.Count; i++)
                newRow[i] = j[i].NullItem ? null : j[i].ItemValue;
            dt.Rows.Add(newRow);
        }

        return dt;
    }



}
