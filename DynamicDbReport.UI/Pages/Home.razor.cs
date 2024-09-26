using DynamicDbReport.DTO.Models.SQLModels;
using DynamicDbReport.UI.PrivateServices;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using DynamicDbReport.DTO.Shared;
using System.Text;
using Blazored.LocalStorage;
using Microsoft.FluentUI.AspNetCore.Components;
using System.IO;
using System.IO.Pipelines;

namespace DynamicDbReport.UI.Pages;

public partial class Home
{
    [Inject] private HttpClientHelper http { get; set; }
    [Inject] private IJSRuntime _js { get; set; }
    [Inject] private IToastService toast { get; set; }
    [Inject] private ILocalStorageService localStorage { get; set; }

    bool showAsGrid = true;
    bool hasResult = false;
    bool _processing = false;
    string selectedTab = "TabOne";
    private ExecuteScriptRequest executeScript;
    private ExecuteScriptResponse executeRespones;
    protected override void OnInitialized()
    {
        executeScript = new();
        executeRespones = new() { ResponseData = new() { Columns = [], Rows = [] } };
    }

    private void ShowErrorOnExecute(string errorMessage)
    {
        toast.ShowError(errorMessage);
        selectedTab = "TabOne";
        hasResult = false;
        StateHasChanged();
    }

    private async Task ExecuteScripts()
    {
        if (string.IsNullOrEmpty(executeScript.QueryToExecute) || executeScript.QueryToExecute == "")
        {
            ShowErrorOnExecute("No query to execute");
            return;
        }

        if (!await localStorage.ContainKeyAsync("dbMember"))
        {
            ShowErrorOnExecute("Login again");
            return;
        }
        executeScript.Credential = await localStorage.GetItemAsync<CredentialRequest>("dbMember");

        if (executeScript?.Credential is null || string.IsNullOrEmpty(executeScript.Credential.DbName) || executeScript.Credential.DbName == "")
        {
            ShowErrorOnExecute("Select DB first");
            return;
        }

        _processing = true;
        executeRespones = await http.HttpClientReceiveAsync<ExecuteScriptResponse>(HttpMethod.Post, "SQLFunctions/ExecuteScript", executeScript.ToJsonString());
        _processing = false;

        if (executeRespones?.ResponseData?.Columns is null || executeRespones.ErrorException is not null || !executeRespones.SuccessAction)
        {
            ShowErrorOnExecute(executeRespones?.ErrorException?.ErrorMessage ?? "Cannot connect to server");
            return;
        }

        hasResult = true;
        selectedTab = "TabTwo";

        StateHasChanged();
    }

    private async Task ExportToExcel()
    {
        if (executeRespones?.ResponseData?.Columns is null || executeRespones.ResponseData.Columns.Count == 0 || executeRespones?.ResponseData?.Rows is null || executeRespones.ResponseData.Rows.Count == 0)
        {
            toast.ShowError("No data to show");
            return;
        }

        byte[] excellBytes;
        var stream = new MemoryStream();
        using (var package = new ExcelPackage(stream))
        {
            var workSheet = package.Workbook.Worksheets.Add("DD_R_1");
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            for (int i = 0; i < executeRespones.ResponseData.Columns.Count; i++)
            {
                workSheet.Cells[1, i + 1].Value = executeRespones.ResponseData.Columns[i].ColumnName;
            }

            for (int row = 0; row < executeRespones.ResponseData.Rows.Count; row++)
            {
                for (int i = 0; i < executeRespones.ResponseData.Rows[row].Count; i++)
                {
                    workSheet.Cells[row + 2, i + 1].Value = executeRespones.ResponseData.Rows[row][i];
                }
            }

            package.Save();
            excellBytes = package.GetAsByteArray();
        }

        await _js.InvokeVoidAsync("downloadFileFromStream2", $"DataExport-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx", Convert.ToBase64String(excellBytes));
    }

    private async Task ExportQuery()
    {
        if (string.IsNullOrEmpty(executeScript.QueryToExecute) || executeScript.QueryToExecute == "")
        {
            toast.ShowError("No query to export");
            return;
        }

        using MemoryStream memoryStream = new MemoryStream();
        using StreamWriter writer = new StreamWriter(memoryStream, Encoding.UTF8);
        writer.WriteLine(executeScript.QueryToExecute);
        writer.Flush();

        await _js.InvokeVoidAsync("downloadFileFromStream2", $"QueryExport-{DateTime.Now:yyyyMMddHHmmssfff}.sql", Convert.ToBase64String(memoryStream.ToArray()));
    }

    private async Task UploadFiles(FluentInputFileEventArgs file)
    {
        if (file is null)
        {
            toast.ShowError("No file selected");
            return;
        }
        
        using MemoryStream ms = new();
        byte[] responseBytes = [];
        try
        {
            await file.Stream.CopyToAsync(ms);
            responseBytes = ms.ToArray();
        }
        catch (Exception x)
        {
            toast.ShowError(x.Message);
            return;
        }
        executeScript.QueryToExecute = Encoding.UTF8.GetString(Encoding.Convert(Encoding.UTF8, Encoding.UTF8, responseBytes));
        StateHasChanged();
    }

}
