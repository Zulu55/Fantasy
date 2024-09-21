﻿using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Fantasy.Frontend.AuthenticationProviders;

public class AuthenticationProviderTest : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await Task.Delay(1000);

        var anonimous = new ClaimsIdentity();
        var user = new ClaimsIdentity(authenticationType: "test");
        var admin = new ClaimsIdentity(new List<Claim>
        {
            new Claim("FirstName", "Juan"),
            new Claim("LastName", "Zulu"),
            new Claim(ClaimTypes.Name, "zulu@yopmail.com"),
            new Claim(ClaimTypes.Role, "Admin")
        },
        authenticationType: "test");

        return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(admin)));
    }
}