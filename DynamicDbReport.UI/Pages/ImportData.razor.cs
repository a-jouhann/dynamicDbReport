using Blazored.LocalStorage;
using DynamicDbReport.DTO.Models.Public;
using DynamicDbReport.DTO.Models.SQLModels;
using DynamicDbReport.DTO.Shared;
using DynamicDbReport.UI.PrivateServices;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using OfficeOpenXml;
using System.Data;
namespace DynamicDbReport.UI.Pages;

public partial class ImportData
{
    [Inject] private HttpClientHelper http { get; set; }
    [Inject] private IToastService toast { get; set; }
    [Inject] private ILocalStorageService localStorage { get; set; }

    private static readonly string[] SQLInjectItems = { "--", ";--", ";", "/*", "*", "#", "-", "*/", "@@", "@", "char", "nchar", "varchar", "nvarchar", "alter", "begin", "cast", "create", "cursor", "declare", "delete", "drop", "end", "exec", "execute", "fetch", "insert", "kill", "select", "sys", "sysobjects", "syscolumns", "table", "update", "dec", "proc" };

    string fileName = "";

    FluentInputFile myFileByStream = default!;
    int progressPercent;
    string progressTitle;
    bool uploadComplete = false;
    string tableName = "";
    private List<ExecuteScriptResponse.ColumnDetail> fileColumns = [];
    private List<List<string>> fileRows = [];

    void OnCompleted(IEnumerable<FluentInputFileEventArgs> files)
    {
        progressPercent = myFileByStream!.ProgressPercent;
        progressTitle = myFileByStream!.ProgressTitle;
    }

    private async Task OnFileUploadedAsync(FluentInputFileEventArgs file)
    {
        fileName = file.Name;
        tableName = fileName.Split('.')[0];
        foreach (var j in SQLInjectItems) tableName = tableName.Replace(j, "");

        progressPercent = file.ProgressPercent;
        progressTitle = file.ProgressTitle;

        using MemoryStream ms = new();
        try
        {
            await file.Stream.CopyToAsync(ms);
        }
        catch (Exception x)
        {
            toast.ShowError(x.Message);
            return;
        }

        using var package = new ExcelPackage(ms);
        var worksheet = package.Workbook.Worksheets[0];
        int maxColumns = worksheet.Dimension.End.Column;

        for (int i = 1; i <= maxColumns; i++)
        {
            var cell = worksheet.Cells[1, i];
            if (cell?.Value is null) continue;
            fileColumns.Add(new() { ColumnName = cell.Text, ColumnType = "NVARCHAR", Length = 255 });
        }

        for (int r = 2; r <= worksheet.Dimension.End.Row; r++)
        {
            List<string> currentRow = [];
            for (int c = 1; c <= maxColumns; c++)
            {
                var cell = worksheet.Cells[r, c];
                if (cell?.Value is null)
                {
                    currentRow.Add("NULL");
                    continue;
                }
                currentRow.Add(cell.Text);
            }
            fileRows.Add(currentRow);
        }

        for (int c = 0; c < maxColumns; c++)
        {
            var currentItemsMax = fileRows.Where(i => i.Count >= c).Select(i => i[c].Length).Max();
            fileColumns[c].Length = currentItemsMax;
        }

        StateHasChanged();
    }

    private async Task ImportTable()
    {
        if (!uploadComplete)
        {
            toast.ShowError("upload your file!");
            return;
        }

        if (string.IsNullOrEmpty(tableName))
        {
            toast.ShowError("Set table name");
            return;
        }

        if (fileColumns is null || fileColumns.Count == 0 || fileColumns.Any(i => SQLInjectItems.Any(s => i.ColumnName.Contains(s) || i.ColumnType.Contains(s))))
        {
            toast.ShowError("columns are not OK to insert");
            return;
        }

        DataTable dt = new();
        foreach (var j in fileColumns)
            dt.Columns.Add(j.ColumnName);

        foreach (var j in fileRows)
        {
            var newRow = dt.NewRow();
            for (int i = 0; i < j.Count; i++)
                newRow[i] = j;
            dt.Rows.Add(newRow);
        }

        var credential = await localStorage.GetItemAsync<CredentialRequest>("dbMember");
        var executeRespones = await http.HttpClientReceiveAsync<PublicActionResponse>(HttpMethod.Post, "SQLFunctions/ImportTable", new ImportTableRequest { Credential = credential , dataTable = dt, TableName = tableName }.ToJsonString());
        
        if (executeRespones is null || executeRespones.ErrorException is not null || !executeRespones.SuccessAction)
        {
            toast.ShowError(executeRespones?.ErrorException?.ErrorMessage ?? "Cannot connect to server");
            return;
        }

        toast.ShowSuccess("Table imported successfully");

    }


}
