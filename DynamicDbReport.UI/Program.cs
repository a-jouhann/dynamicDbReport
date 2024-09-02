using Blazored.LocalStorage;
using DynamicDbReport.UI;
using DynamicDbReport.UI.PrivateServices;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorageAsSingleton();
builder.Services.AddFluentUIComponents();
builder.Services.AddScoped<CustomAuthentication>();
builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<CustomAuthentication>());
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["APIAddress"]) });
builder.Services.AddScoped<HttpClientHelper>();

await builder.Build().RunAsync();
