using DynamicDbReport.DTO.Models.Public;
using DynamicDbReport.DTO.Models.SQLModels;
using DynamicDbReport.DTO.Shared;
using DynamicDbReport.UI.PrivateServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.FluentUI.AspNetCore.Components;

namespace DynamicDbReport.UI.Pages.Auth;

public partial class Login
{
    [Inject] private HttpClientHelper http { get; set; }
    [Inject] private IToastService toast { get; set; }
    [Inject] private NavigationManager navigate { get; set; }
    [Inject] private AuthenticationStateProvider _authenticationStateProvider { get; set; }
    [Inject] private IMessageService messageService { get; set; }

    CredentialRequest _loginModel = new();
    bool _processing = false;
    List<EngineName> enginesList = ((EngineName[])Enum.GetValues(typeof(EngineName))).ToList();

    private async Task CheckCredential()
    {
        _processing = true;
        var fetchData = await http.HttpClientReceiveAsync<CheckCredentialResponse>(HttpMethod.Post, "SQLFunctions/CheckDBConnection", _loginModel.ToJsonString());
        if (fetchData is null || !fetchData.SuccessAction || !fetchData.ResponseData)
        {
            toast.ShowError(fetchData?.ErrorException?.ErrorMessage ?? "Cannot connect to server");
            return;
        }

        await ((CustomAuthentication)_authenticationStateProvider).MarkUserAsAuthenticated(_loginModel);
        _processing = false;
        toast.ShowSuccess("User access to use db");
        messageService.Clear();
        navigate.NavigateTo("/");
    }


}
