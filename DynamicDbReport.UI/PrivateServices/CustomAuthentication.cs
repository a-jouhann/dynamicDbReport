using Blazored.LocalStorage;
using DynamicDbReport.DTO.Models.SQLModels;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace DynamicDbReport.UI.PrivateServices;

internal class CustomAuthentication(ILocalStorageService _localStorage) : AuthenticationStateProvider
{
    private ILocalStorageService localStorage { get; set; } = _localStorage;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();
        try
        {
            if (await localStorage.ContainKeyAsync("dbMember"))
            {
                var memberDetail = await localStorage.GetItemAsync<CredentialRequest>("dbMember");
                identity = GetClaimsIdentity(memberDetail);
            }
        }
        catch { }
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task MarkUserAsAuthenticated(CredentialRequest user)
    {
        var identity = GetClaimsIdentity(user);
        await localStorage.SetItemAsync("dbMember", user);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await localStorage.RemoveItemAsync("dbMember");
        await localStorage.RemoveItemAsync("dbName");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
    }

    private ClaimsIdentity GetClaimsIdentity(CredentialRequest user)
    {
        if (user is null) 
            return new();

        List<Claim> ClaimList =
        [
            new Claim(ClaimTypes.Uri, user.ServerAddress),
            new Claim(ClaimTypes.Name, user.Username),            
            new Claim(ClaimTypes.Hash, user.Password)
        ];

        return new ClaimsIdentity(ClaimList, "apiauth_type");
    }

}