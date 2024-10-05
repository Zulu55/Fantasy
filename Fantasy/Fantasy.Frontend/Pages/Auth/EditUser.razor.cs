using Fantasy.Frontend.Repositories;
using Fantasy.Frontend.Services;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Net;

namespace Fantasy.Frontend.Pages.Auth;

[Authorize]
public partial class EditUser
{
    private User? user;
    private List<Country>? countries;
    private bool loading = true;
    private string? imageUrl;

    private Country selectedCountry = new();

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ILoginService LoginService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserAsyc();
        await LoadCountriesAsync();

        selectedCountry = user!.Country!;

        if (!string.IsNullOrEmpty(user!.Photo))
        {
            imageUrl = user.Photo;
            user.Photo = null;
        }
    }

    private void ShowModal()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
        DialogService.Show<ChangePassword>(Localizer["ChangePassword"], closeOnEscapeKey);
    }

    private async Task LoadUserAsyc()
    {
        var responseHttp = await Repository.GetAsync<User>($"/api/accounts");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/");
                return;
            }
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }
        user = responseHttp.Response;
        loading = false;
    }

    private void ImageSelected(string imagenBase64)
    {
        user!.Photo = imagenBase64;
        imageUrl = null;
    }

    private async Task LoadCountriesAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Country>>("/api/countries/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message], Severity.Error);
            return;
        }
        countries = responseHttp.Response;
    }

    private void CountryChanged(Country country)
    {
        selectedCountry = country;
    }

    private async Task<IEnumerable<Country>> SearchCountries(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return countries!;
        }

        return countries!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task SaveUserAsync()
    {
        var responseHttp = await Repository.PutAsync<User, TokenDTO>("/api/accounts", user!);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        await LoginService.LoginAsync(responseHttp.Response!.Token);
        Snackbar.Add(Localizer["RecordSavedOk"], Severity.Success);
        NavigationManager.NavigateTo("/");
    }

    private void ReturnAction()
    {
        NavigationManager.NavigateTo("/");
    }
}