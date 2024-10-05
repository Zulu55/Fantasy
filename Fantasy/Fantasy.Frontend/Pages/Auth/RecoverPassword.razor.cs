using Fantasy.Frontend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Fantasy.Frontend.Pages.Auth;

public partial class RecoverPassword
{
    private EmailDTO emailDTO = new();
    private bool loading;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

    private async Task SendRecoverPasswordEmailTokenAsync()
    {
        emailDTO.Language = System.Globalization.CultureInfo.CurrentCulture.Name.Substring(0, 2);
        loading = true;
        var responseHttp = await Repository.PostAsync("/api/accounts/RecoverPassword", emailDTO);
        loading = false;

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        MudDialog.Cancel();
        NavigationManager.NavigateTo("/");
        Snackbar.Add(Localizer["RecoverPasswordMessage"], Severity.Success);
    }
}