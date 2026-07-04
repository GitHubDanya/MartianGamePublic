using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ServerAlphaWebsite.Classes;
using ServerAlphaWebsite.ServerStorage;
using ServerAlphaWebsite.Services;
using System.Security.Claims;

namespace ServerAlphaWebsite;

public abstract class GamePageBase : ComponentBase
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthStateTask { get; set; } = null!;

    [Inject] protected UserInfoStorage userInfoStorage { get; set; } = null!;
    [Inject] protected NavigationManager navigationManager { get; set; } = null!;

    protected User CurrentUser { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateTask;
        var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        CurrentUser = userInfoStorage.LogUser(userId);
    }

    protected void ChangeStage(GameStage stage)
    {
        CurrentUser.CurrentStage = stage;
        navigationManager.NavigateTo(StageValidationService.GetUrlForStage(stage));
    }
}
