using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DynamicDbReport.API.Controllers;

[Route("[controller]")]
[ApiController]
public class SQLFunctionsController : ControllerBase
{

    [HttpGet("Index")]
    public ActionResult Index()
    {
        return Content($"<html><header><title>DD Report</title></header><body><h1>Dynamic db report API</h1></body></html>", "text/html", Encoding.UTF8);
    }


}
