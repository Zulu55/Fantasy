using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend.Pages.Tournaments;

public partial class TournamentForm
{
    private EditContext editContext = null!;

    protected override void OnInitialized()
    {
        editContext = new(TournamentDTO);
    }

    [EditorRequired, Parameter] public TournamentDTO TournamentDTO { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    public bool FormPostedSuccessfully { get; set; } = false;

    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    private string? imageUrl;
    private string? isActiveMessage;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!string.IsNullOrEmpty(TournamentDTO.Image))
        {
            imageUrl = TournamentDTO.Image;
            TournamentDTO.Image = null;
        }
        isActiveMessage = TournamentDTO.IsActive ? Localizer["TournamentActive"] : Localizer["TournamentInactive"];
    }

    private void OnToggledChanged(bool toggled)
    {
        TournamentDTO.IsActive = toggled;
        isActiveMessage = TournamentDTO.IsActive ? Localizer["TournamentActive"] : Localizer["TournamentInactive"];
    }

    private void ImageSelected(string imagenBase64)
    {
        TournamentDTO.Image = imagenBase64;
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