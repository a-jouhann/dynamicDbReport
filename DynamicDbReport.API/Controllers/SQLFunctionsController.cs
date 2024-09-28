using DynamicDbReport.DTO.Models.Public;
using DynamicDbReport.DTO.Models.SQLModels;
using DynamicDbReport.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DynamicDbReport.API.Controllers;

[Route("[controller]")]
[ApiController]
public class SQLFunctionsController(DBService _db) : ControllerBase
{



    [HttpGet("Index")]
    public ActionResult Index()
    {
        return Content($"<html><header><title>DD Report</title></header><body><h1>Dynamic db report API</h1></body></html>", "text/html", Encoding.UTF8);
    }


    [HttpPost("CheckDBConnection")]
    public ActionResult<CheckCredentialResponse> CheckDBConnection(CredentialRequest requestModel)
    {
        return _db.CheckDBConnection(requestModel);
    }

    [HttpPost("DBNameList")]
    public ActionResult<DatabaseNameListResponse> DBNameList(CredentialRequest requestModel)
    {
        return _db.DBNameList(requestModel);
    }

    [HttpPost("ExecuteScript")]
    public ActionResult<ExecuteScriptResponse> ExecuteScript(ExecuteScriptRequest requestModel)
    {
        return _db.ExecuteScript(requestModel);
    }

    [HttpPost("ImportTable")]
    public async Task<ActionResult<PublicActionResponse>> ImportTable(ImportTableRequest requestModel)
    {
        return await _db.ImportTable(requestModel);
    }



}
