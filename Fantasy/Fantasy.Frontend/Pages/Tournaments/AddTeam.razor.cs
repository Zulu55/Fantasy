using Fantasy.Frontend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Fantasy.Frontend.Pages.Tournaments;

public partial class AddTeam
{
    private TournamentTeamDTO? tournamentTeamDTO;
    private AddTeamForm? addTeamForm;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [Parameter] public int Id { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        tournamentTeamDTO = new TournamentTeamDTO()
        {
            TournamentId = Id,
        };
    }

    private async Task AddAsync()
    {
        var responseHttp = await Repository.PostAsync("api/TournamentTeams/full", tournamentTeamDTO);

        if (responseHttp.Error)
        {
            var menssageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[menssageError!], Severity.Error);
            return;
        }

        Return();
        Snackbar.Add(Localizer["RecordCreatedOk"], Severity.Success);
    }

    private void Return()
    {
        addTeamForm!.FormPostedSuccessfully = true;
        NavigationManager.NavigateTo($"/tournament/teams/{Id}");
    }
}