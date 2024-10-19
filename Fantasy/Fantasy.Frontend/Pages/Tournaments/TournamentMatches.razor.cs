using Fantasy.Frontend.Repositories;
using Fantasy.Frontend.Shared;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Net;

namespace Fantasy.Frontend.Pages.Tournaments;

[Authorize(Roles = "Admin")]
public partial class TournamentMatches
{
    private Tournament? tournament;
    private List<Match>? matches;
    private MudTable<Match> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrlTournament = "api/tournaments";
    private const string baseUrlMatch = "api/matches";
    private string infoFormat = "{first_item}-{last_item} de {all_items}";

    [Parameter] public int TournamentId { get; set; }

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        await LoadTotalRecords();
    }

    private async Task<bool> LoadTournamentAsync()
    {
        var responseHttp = await Repository.GetAsync<Tournament>($"{baseUrlTournament}/{TournamentId}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/tournaments");
                return false;
            }

            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message], Severity.Error);
            return false;
        }
        tournament = responseHttp.Response;
        return true;
    }

    private async Task<bool> LoadTotalRecords()
    {
        loading = true;
        if (tournament is null)
        {
            var ok = await LoadTournamentAsync();
            if (!ok)
            {
                NoTournament();
                return false;
            }
        }

        var url = $"{baseUrlMatch}/totalRecordsPaginated/?id={TournamentId}";
        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }
        var responseHttp = await Repository.GetAsync<int>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return false;
        }
        totalRecords = responseHttp.Response;
        loading = false;
        return true;
    }

    private async Task<TableData<Match>> LoadListAsync(TableState state, CancellationToken cancellationToken)
    {
        int page = state.Page + 1;
        int pageSize = state.PageSize;
        var url = $"{baseUrlMatch}/paginated?id={TournamentId}&page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<Match>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return new TableData<Match> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<Match> { Items = [], TotalItems = 0 };
        }
        return new TableData<Match>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadAsync();
        await table.ReloadServerData();
    }

    private void ReturnAction()
    {
        NavigationManager.NavigateTo("/tournaments");
    }

    private async Task ShowModalAsync(int id = 0, bool isEdit = false)
    {
        var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
        IDialogReference? dialog;
        if (isEdit)
        {
            var parameters = new DialogParameters
                {
                    { "Id", id }
                };
            dialog = DialogService.Show<EditMatch>($"{Localizer["Edit"]} {Localizer["Match"]}", parameters, options);
        }
        else
        {
            var parameters = new DialogParameters
            {
                { "Id", TournamentId }
            };
            dialog = DialogService.Show<AddMatch>(Localizer["AddMatchToTournament"], parameters, options);
        }

        var result = await dialog.Result;
        if (result!.Canceled)
        {
            await LoadAsync();
            await table.ReloadServerData();
        }
    }

    private void NoTournament()
    {
        NavigationManager.NavigateTo("/tournaments");
    }

    private async Task DeleteAsync(Match match)
    {
        var parameters = new DialogParameters
        {
            { "Message", string.Format(Localizer["DeleteConfirm"], Localizer["Match"], $"{match.Local.Name} Vs. {match.Visitor.Name}") }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = DialogService.Show<ConfirmDialog>(Localizer["Confirmation"], parameters, options);
        var result = await dialog.Result;
        if (result!.Canceled)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"{baseUrlMatch}/{match.Id}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        await LoadAsync();
        await table.ReloadServerData();
        Snackbar.Add(Localizer["RecordDeletedOk"], Severity.Success);
    }
}