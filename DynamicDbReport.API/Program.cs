using DynamicDbReport.API.PrivateServices;
using DynamicDbReport.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<DBService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowedCORS", policy => { policy.SetIsOriginAllowed(CORSAuthorization.IsOriginAllowed).AllowAnyHeader().AllowAnyMethod().AllowCredentials(); });
});

var app = builder.Build();

app.UseCors("AllowedCORS");

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    await next.Invoke();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
