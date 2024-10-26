using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Fantasy.Frontend.Pages.Groups;

public partial class GroupForm
{
    private string? imageUrl;
    private string? isActiveMessage;
    private EditContext editContext = null!;
    private Tournament selectedTournament = new();
    private List<Tournament>? tournaments;

    protected override async Task OnInitializedAsync()
    {
        editContext = new(GroupDTO);
        await LoadTournamentAsync();
    }

    [EditorRequired, Parameter] public GroupDTO GroupDTO { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    public bool FormPostedSuccessfully { get; set; } = false;

    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!string.IsNullOrEmpty(GroupDTO.Image))
        {
            imageUrl = GroupDTO.Image;
            GroupDTO.Image = null;
        }
        isActiveMessage = GroupDTO.IsActive ? $"{Localizer["Group"]} {Localizer["Active"]}" : $"{Localizer["Group"]} {Localizer["Inactive"]}";
    }

    private void OnToggledChanged(bool toggled)
    {
        GroupDTO.IsActive = toggled;
        isActiveMessage = GroupDTO.IsActive ? $"{Localizer["Group"]} {Localizer["Active"]}" : $"{Localizer["Group"]} {Localizer["Inactive"]}";
    }

    private void OnInvalidSubmit(EditContext editContext)
    {
        var messages = editContext.GetValidationMessages();

        foreach (var message in messages)
        {
            Snackbar.Add(Localizer[message!], Severity.Error);
        }
    }

    private async Task LoadTournamentAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Tournament>>("/api/tournaments/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            return;
        }

        tournaments = responseHttp.Response;
    }

    private async Task<IEnumerable<Tournament>> SearchTournament(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return tournaments!;
        }

        return tournaments!
            .Where(x => x.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private void TournamentChanged(Tournament tournament)
    {
        selectedTournament = tournament;
        GroupDTO.TournamentId = tournament.Id;
    }

    private void ImageSelected(string imagenBase64)
    {
        GroupDTO.Image = imagenBase64;
        imageUrl = null;
    }

    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        var formWasEdited = editContext.IsModified();

        if (!formWasEdited || FormPostedSuccessfully)
        {
            return;
        }

        var result = await SweetAlertService.FireAsync(new SweetAlertOptions
        {
            Title = Localizer["Confirmation"],
            Text = Localizer["LeaveAndLoseChanges"],
            Icon = SweetAlertIcon.Warning,
            ShowCancelButton = true,
            CancelButtonText = Localizer["Cancel"],
        });

        var confirm = !string.IsNullOrEmpty(result.Value);
        if (confirm)
        {
            return;
        }

        context.PreventNavigation();
    }
}