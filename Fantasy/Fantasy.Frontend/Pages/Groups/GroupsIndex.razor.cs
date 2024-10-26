using Fantasy.Frontend.Repositories;
using Fantasy.Frontend.Shared;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Net;
using System.Security.Cryptography;
using Fantasy.Frontend.Helpers;

namespace Fantasy.Frontend.Pages.Groups;

[Authorize(Roles = "Admin, User")]
public partial class GroupsIndex
{
    private List<Group>? Groups { get; set; }
    private MudTable<Group> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/groups";
    private string infoFormat = "{first_item}-{last_item} => {all_items}";
    private string username = string.Empty;

    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private IClipboardService ClipboardService { get; set; } = null!;
    [Inject] private IStringLocalizer<Parameters> Parameters { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserNameAsync();
        await LoadTotalRecordsAsync();
    }

    private async Task LoadUserNameAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            username = user.Identity.Name!;
        }
    }

    private async Task AdminUsersGroupAsync(Group group)
    {
        {
            var options = new DialogOptions()
            {
                CloseOnEscapeKey = true,
                CloseButton = true,
                MaxWidth = MaxWidth.Large,
                FullWidth = true
            };
            var parameters = new DialogParameters
            {
                { "GroupId", group.Id },
            };
            var dialog = DialogService.Show<UsersGroup>(@Localizer["AdminUsersGroup"], parameters, options);
            await dialog.Result;
        }
    }

    private async Task CopyInvitationAsync(Group group)
    {
        var joinURL = $"{Parameters["URLFront"]}/groups/join/?code={group!.Code}";
        await ClipboardService.CopyToClipboardAsync(joinURL);
        var text = string.Format(Localizer["InvitationURLCopied"], group!.Name);
        Snackbar.Add(text, Severity.Success);
    }

    private void GroupDetails(Group group)
    {
        NavigationManager.NavigateTo($"/groups/details/{group.Id}/false");
    }

    private async Task LoadTotalRecordsAsync()
    {
        loading = true;
        var url = $"{baseUrl}/totalRecordsPaginated";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"?filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<int>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message], Severity.Error);
            return;
        }

        totalRecords = responseHttp.Response;
        loading = false;
    }

    private async Task<TableData<Group>> LoadListAsync(TableState state, CancellationToken cancellationToken)
    {
        int page = state.Page + 1;
        int pageSize = state.PageSize;
        var url = $"{baseUrl}/paginated/?page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<Group>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message], Severity.Error);
            return new TableData<Group> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<Group> { Items = [], TotalItems = 0 };
        }
        return new TableData<Group>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
    }

    private async Task ShowModalJoinAsync()
    {
        var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
        var dialog = DialogService.Show<JoinGroup>($"{Localizer["JoinExistingGroup"]}", options);

        var result = await dialog.Result;
        if (result!.Canceled)
        {
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }
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
            dialog = DialogService.Show<GroupEdit>($"{Localizer["Edit"]} {Localizer["Group"]}", parameters, options);
        }
        else
        {
            dialog = DialogService.Show<GroupCreate>($"{Localizer["New"]} {Localizer["Group"]}", options);
        }

        var result = await dialog.Result;
        if (result!.Canceled)
        {
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }
    }

    private async Task DeleteAsync(Group team)
    {
        var parameters = new DialogParameters
            {
                { "Message", string.Format(Localizer["DeleteConfirm"], Localizer["Group"], team.Name) }
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = DialogService.Show<ConfirmDialog>(Localizer["Confirmation"], parameters, options);
        var result = await dialog.Result;
        if (result!.Canceled)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"{baseUrl}/{team.Id}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/groups");
            }
            else
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(Localizer[message], Severity.Error);
            }
            return;
        }
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
        Snackbar.Add(Localizer["RecordDeletedOk"], Severity.Success);
    }
}