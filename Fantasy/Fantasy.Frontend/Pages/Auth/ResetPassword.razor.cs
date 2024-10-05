using Fantasy.Frontend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Fantasy.Frontend.Pages.Auth;

public partial class ResetPassword
{
    private ResetPasswordDTO resetPasswordDTO = new();
    private bool loading;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Parameter, SupplyParameterFromQuery] public string Token { get; set; } = string.Empty;

    private async Task ChangePasswordAsync()
    {
        resetPasswordDTO.Token = Token;
        loading = true;
        var responseHttp = await Repository.PostAsync("/api/accounts/ResetPassword", resetPasswordDTO);
        loading = false;
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        Snackbar.Add(Localizer["PasswordRecoveredMessage"], Severity.Success);
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
        DialogService.Show<Login>(Localizer["Login"], closeOnEscapeKey);
    }
}