using Fantasy.Frontend.Repositories;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Fantasy.Frontend.Pages.Auth;

public partial class ConfirmEmail
{
    private string? message;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string UserId { get; set; } = string.Empty;
    [Parameter, SupplyParameterFromQuery] public string Token { get; set; } = string.Empty;

    protected async Task ConfirmAccountAsync()
    {
        var responseHttp = await Repository.GetAsync($"/api/accounts/ConfirmEmail/?userId={UserId}&token={Token}");
        if (responseHttp.Error)
        {
            message = await responseHttp.GetErrorMessageAsync();
            NavigationManager.NavigateTo("/");
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        Snackbar.Add(Localizer["ConfirmedEmailMessage"], Severity.Success);
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
        DialogService.Show<Login>(Localizer["Login"], closeOnEscapeKey);
    }
}