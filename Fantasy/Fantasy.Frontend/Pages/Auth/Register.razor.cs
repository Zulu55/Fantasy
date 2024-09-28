using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Frontend.Services;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Enums;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Fantasy.Frontend.Pages.Auth;

public partial class Register
{
    private UserDTO userDTO = new();
    private List<Country>? countries;
    private bool loading;
    private string? imageUrl;
    private string? titleLabel;

    private Country selectedCountry = new();

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILoginService LogInService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Parameter, SupplyParameterFromQuery] public bool IsAdmin { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadCountriesAsync();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        titleLabel = IsAdmin ? Localizer["AdminRegister"] : Localizer["UserRegister"];
    }

    private void ImageSelected(string imageBase64)
    {
        userDTO.Photo = imageBase64;
        imageUrl = null;
    }

    private async Task LoadCountriesAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Country>>("/api/countries/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
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

    private void ReturnAction()
    {
        NavigationManager.NavigateTo("/");
    }

    private async Task CreateUserAsync()
    {
        if (!ValidateForm())
        {
            return;
        }

        userDTO.UserType = UserType.User;
        userDTO.UserName = userDTO.Email;
        userDTO.Country = selectedCountry;
        userDTO.CountryId = selectedCountry.Id;
        userDTO.Language = System.Globalization.CultureInfo.CurrentCulture.Name.Substring(0, 2);

        if (IsAdmin)
        {
            userDTO.UserType = UserType.Admin;
        }

        loading = true;
        var responseHttp = await Repository.PostAsync<UserDTO>("/api/accounts/CreateUser", userDTO);
        loading = false;
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            if (message!.Contains("DuplicateUserName"))
            {
                Snackbar.Add(Localizer["EmailAlreadyExists"], Severity.Error);
                return;
            }
            Snackbar.Add(Localizer[message], Severity.Error);
            return;
        }

        NavigationManager.NavigateTo("/");
        await SweetAlertService.FireAsync(new SweetAlertOptions
        {
            Title = Localizer["Confirmation"],
            Text = Localizer["SendEmailConfirmationMessage"],
            Icon = SweetAlertIcon.Info,
        });
    }

    private bool ValidateForm()
    {
        var hasErrors = false;
        if (string.IsNullOrEmpty(userDTO.FirstName))
        {
            Snackbar.Add(string.Format(Localizer["RequiredField"], string.Format(Localizer["FirstName"])), Severity.Error);
            hasErrors = true;
        }
        if (string.IsNullOrEmpty(userDTO.LastName))
        {
            Snackbar.Add(string.Format(Localizer["RequiredField"], string.Format(Localizer["LastName"])), Severity.Error);
            hasErrors = true;
        }
        if (string.IsNullOrEmpty(userDTO.PhoneNumber))
        {
            Snackbar.Add(string.Format(Localizer["RequiredField"], string.Format(Localizer["PhoneNumber"])), Severity.Error);
            hasErrors = true;
        }
        if (string.IsNullOrEmpty(userDTO.Email))
        {
            Snackbar.Add(string.Format(Localizer["RequiredField"], string.Format(Localizer["Email"])), Severity.Error);
            hasErrors = true;
        }
        if (string.IsNullOrEmpty(userDTO.Password))
        {
            Snackbar.Add(string.Format(Localizer["RequiredField"], string.Format(Localizer["Password"])), Severity.Error);
            hasErrors = true;
        }
        if (string.IsNullOrEmpty(userDTO.PasswordConfirm))
        {
            Snackbar.Add(string.Format(Localizer["RequiredField"], string.Format(Localizer["PasswordConfirm"])), Severity.Error);
            hasErrors = true;
        }
        if (selectedCountry.Id == 0)
        {
            Snackbar.Add(string.Format(Localizer["RequiredField"], string.Format(Localizer["Country"])), Severity.Error);
            hasErrors = true;
        }

        return !hasErrors;
    }
}